using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using NHIdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;
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
            query = query.Where(log => log.UserName!.Contains(userName!));
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
        var sql = """
            with logs as (
              select
                  logs.start_at as request_date,
                  count(logs.id) as request_count,
                  coalesce(avg(logs.duration), 0) as avg_duration,
                  coalesce(max(logs.duration), 0) as max_duration,
                  coalesce(min(logs.duration), 0) as min_duration
              from (
                  select
                      l.id, date_trunc('day', l.start_at) as start_at, l.duration
                  from public.app_audit_logs l
                  where l.start_at >= @startDate and l.start_at < @endDate
              ) logs
              group by request_date
            ),
            date_range as (
              select generate_series(@startDate, date_trunc('day', @endDate), '1 day') as date
            )
            select
              date_range.date as request_date,
              logs.request_count,
              logs.avg_duration,
              logs.max_duration,
              logs.min_duration
            from date_range left join logs on logs.request_date = date_range.date
            order by date_range.date;
            """;
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
            .Where(log => log.StartAt >= startDate && log.StartAt < endDate)
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

    public async Task<PaginatedResponseModel<AppAuditLogDurationStatModel>> StatDurationAsync(DateTime startDate, DateTime endDate) {
        var sql = @"
            select substr(logs.duration, 3) as duration, logs.request_count from (
                select
                    case
                       when duration < 200 then '0: < 200ms'
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
        var durations = await conn.QueryAsync<AppAuditLogDurationStatModel>(sql, new { startDate, endDate});
        var result = new PaginatedResponseModel<AppAuditLogDurationStatModel> {
            Data = durations.ToList()
        };
        return result;
    }

    public async Task<PaginatedResponseModel<AppAuditLogUserStatModel>> StatUserAsync(DateTime startDate, DateTime endDate) {
        var query = Session.Query<AppAuditLog>()
            .Where(log => log.StartAt >= startDate && log.StartAt < endDate)
            .GroupBy(log => log.UserName!)
            .Select(g => new AppAuditLogUserStatModel {
                Username = g.Key,
                RequestCount = g.Count()
            })
            .OrderBy(x => x.RequestCount);
        var data = await query.ToListAsync();

        var userData = new List<AppAuditLogUserStatModel>();

        var addOrMerge = (string username, int requestCount) => {
            var userModel = userData.FirstOrDefault(x => x.Username == username);
            if (userModel == null) {
                userData.Add(new AppAuditLogUserStatModel {
                    Username = username,
                    RequestCount = requestCount
                });
            }
            else {
                userModel.RequestCount += requestCount;
            }
        };

        foreach (var model in data) {
            var idx = model.Username!.IndexOf(':');
            if (idx < 0) {
                addOrMerge(model.Username, model.RequestCount);
            }
            else {
                addOrMerge(model.Username.Substring(0, idx), model.RequestCount);
            }
        }

        var result = new PaginatedResponseModel<AppAuditLogUserStatModel> {
            Data = userData.OrderBy(x => x.RequestCount).ToList()
        };
        return result;
    }

    public async Task<PaginatedResponseModel<AppAuditLogIpStatModel>> StatIpAsync(DateTime startDate, DateTime endDate) {
        var query = Session.Query<AppAuditLog>()
            .Where(log => log.StartAt >= startDate && log.StartAt < endDate)
            .GroupBy(log => log.Ip!)
            .Select(g => new AppAuditLogIpStatModel {
                Ip = g.Key,
                RequestCount = g.Count()
            })
            .OrderByDescending(x => x.RequestCount);
        var result = new PaginatedResponseModel<AppAuditLogIpStatModel> {
            Data = await query.ToListAsync()
        };
        return result;
    }

}
