using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>应用程序日志仓储实现</summary>
public partial class AppLogRepository : HibernateRepository<AppLog, AppLogModel, long>, IAppLogRepository {

    public AppLogRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    /// <summary>搜索 应用程序日志 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppLogModel>> SearchAsync(
        AppLogSearchModel model
    ) {
        var startDate = model.StartDate.GetValueOrDefault(DateTime.Today);
        var endDate = model.EndDate.GetValueOrDefault(DateTime.Today).AddDays(1);
        var query = Session.Query<AppLog>().Where(
            log => log.CreatedAt >= startDate && log.CreatedAt < endDate
        );
        if (model.Level.IsNotNullOrEmpty()) {
            var level = model.Level.ToUpperInvariant();
            query = query.Where(log => log.Level == level);
        }
        var total = await query.LongCountAsync();
        var data = await query.Select(log => new AppLog {
            Id = log.Id,
            CreatedAt = log.CreatedAt,
            Thread = log.Thread,
            Logger = log.Logger,
            Level = log.Level,
            Message = log.Message,
            Exception = string.Empty
        }).OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppLogModel> {
            Total = total,
            Data = Mapper.Map<IList<AppLogModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }
}
