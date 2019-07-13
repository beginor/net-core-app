using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>导航节点（菜单）仓储实现</summary>
    public partial class AppNavItemRepository : HibernateRepository<AppNavItem, long>, IAppNavItemRepository {

        public AppNavItemRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

    }

}
