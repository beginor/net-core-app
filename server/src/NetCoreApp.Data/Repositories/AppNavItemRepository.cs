using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using Beginor.NetCoreApp.Data.Entities;
using System.Threading.Tasks;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>导航节点（菜单）仓储实现</summary>
    public partial class AppNavItemRepository : HibernateRepository<AppNavItem, long>, IAppNavItemRepository {

        public AppNavItemRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public override async Task DeleteAsync(long id) {
            using (var session = SessionFactory.OpenSession()) {
                var entity = await session.GetAsync<AppNavItem>(id);
                if (entity == null) {
                    return;
                }
                entity.IsDeleted = true;
                await session.UpdateAsync(entity);
                await session.FlushAsync();
                session.Clear();
            }
        }

    }

}
