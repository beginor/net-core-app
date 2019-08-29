using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>系统权限仓储实现</summary>
    public partial class AppPrivilegeRepository : HibernateRepository<AppPrivilege, long>, IAppPrivilegeRepository {

        public AppPrivilegeRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

    }

}
