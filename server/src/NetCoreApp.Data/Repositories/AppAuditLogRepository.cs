using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>审计日志仓储实现</summary>
    public partial class AppAuditLogRepository : HibernateRepository<AppAuditLog, long>, IAppAuditLogRepository {

        public AppAuditLogRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

    }

}
