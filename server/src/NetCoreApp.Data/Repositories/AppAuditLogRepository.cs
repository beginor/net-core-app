using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>审计日志仓储实现</summary>
public partial class AppAuditLogRepository : HibernateRepository<AppAuditLog, AppAuditLogModel, long>, IAppAuditLogRepository {

    public AppAuditLogRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    public async Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
        AppAuditLogSearchModel model
    ) {
        var startDate = model.StartDate.GetValueOrDefault(DateTime.Today);
        var endDate = model.EndDate.GetValueOrDefault(DateTime.Today).AddDays(1);
        var query = Session.Query<AppAuditLog>().Where(
            log => log.StartAt >= startDate && log.StartAt < endDate
        );

        if (model.UserName.IsNotNullOrEmpty()) {
            var userName = model.UserName;
            query = query.Where(log => log.UserName.Contains(userName));
        }
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppAuditLogModel> {
            Total = total,
            Data = Mapper.Map<IList<AppAuditLogModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task<PaginatedResponseModel<AppAuditLogTrafficStatModel>> StatTrafficAsync(DateTime startDate, DateTime endDate) {
        var sql = @"
            with d as (
                select generate_series(@startDate, @endDate, '1 day') as day
            )
            select
                date_trunc('day', d.day) as request_date,
                count(l.*) as request_count,
                coalesce(avg(l.duration), 0) as avg_duration,
                coalesce(max(l.duration), 0) as max_duration,
                coalesce(min(l.duration), 0) as min_duration
            from d
            left join public.app_audit_logs l on l.start_at::date = d.day
            group by d.day;
        ";
        var conn = Session.Connection;
        var trafics = await conn.QueryAsync<AppAuditLogTrafficStatModel>(
            sql, new { startDate, endDate }
        );
        var result = new PaginatedResponseModel<AppAuditLogTrafficStatModel> {
            Data = trafics.ToList()
        };
        return result;
    }

    public async Task<PaginatedResponseModel<AppAuditLogStatusStatModel>> StatStatusAsync(DateTime startDate, DateTime endDate) {
        var query = Session.Query<AppAuditLog>()
            .Where(log => log.StartAt <= startDate && log.StartAt < endDate)
            .GroupBy(log => log.ResponseCode)
            .Select(g => new AppAuditLogStatusStatModel {
                StatusCode = g.Key,
                RequestCount = g.Count()
            }).OrderBy(x => x.StatusCode);
        var result = new PaginatedResponseModel<AppAuditLogStatusStatModel> {
            Data = await query.ToListAsync()
        };
        return result;
    }
    
    public async Task<PaginatedResponseModel<AppAuditlogDurationStatModel>> StatsDurationAsync(DateTime startDate, DateTime endDate) {
        var sql = @"
            select substr(logs.duration, 3) as duration, logs.request_count from (
                select
                    case
                       when duration < 500 then '1: < 500ms'
                       when duration < 1000 then '2: 500ms ~ 1s'
                       when duration < 2000 then '3: 1s ~ 2s'
                       when duration < 5000 then '4: 2s ~ 5s'
                       when duration >= 5000 then '5: > 5s'
                       end as duration,
                    count(*) as request_count
                from public.app_audit_logs
                where (start_at >= @startDate and start_at < @endDate)
                group by 1
            ) as logs
            order by logs.duration;
        ";
        var conn = Session.Connection;
        var durations = await conn.QueryAsync<AppAuditlogDurationStatModel>(sql, new { startDate, endDate});
        var result = new PaginatedResponseModel<AppAuditlogDurationStatModel> {
            Data = durations.ToList()
        };
        return result;
    }
}
