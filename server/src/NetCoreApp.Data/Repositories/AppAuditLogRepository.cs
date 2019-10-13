using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>审计日志仓储实现</summary>
    public partial class AppAuditLogRepository : HibernateRepository<AppAuditLog, AppAuditLogModel, long>, IAppAuditLogRepository {

        public AppAuditLogRepository(ISessionFactory sessionFactory, IMapper mapper) : base(sessionFactory, mapper) { }

        public async Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
            AppAuditLogSearchModel model
        ) {
            using (var session = OpenSession()) {
                var query = session.Query<AppAuditLog>();
                if (model.RequestDate.HasValue) {
                    var startTime = model.RequestDate.Value;
                    var endTime = startTime.AddDays(1);
                    query = query.Where(
                        log => log.StartAt >= startTime && log.StartAt < endTime
                    );
                }
                if (model.UserName.IsNotNullOrEmpty()) {
                    query = query.Where(log => log.UserName.Contains(model.UserName));
                }
                var total = await query.LongCountAsync();
                var data = await query.OrderByDescending(e => e.Id)
                    .Skip(model.Skip).Take(model.Take)
                    .ToListAsync();
                return new PaginatedResponseModel<AppAuditLogModel> {
                    Total = total,
                    Data = Mapper.Map<IList<AppAuditLogModel>>(data),
                    Skip = model.Skip,
                    Take = model.Take
                };
            }
        }
    }

}
