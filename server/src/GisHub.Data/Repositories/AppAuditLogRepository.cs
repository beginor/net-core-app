using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories; 

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
}
