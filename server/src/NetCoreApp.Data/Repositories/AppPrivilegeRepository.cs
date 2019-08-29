using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>系统权限仓储实现</summary>
    public partial class AppPrivilegeRepository : HibernateRepository<AppPrivilege, long>, IAppPrivilegeRepository {

        public AppPrivilegeRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        /// <summary>是否存在指定名称的权限</summary>
        public async Task<bool> ExistsAsync(string name) {
            using (var session = OpenSession()) {
                var exists = await session.Query<AppPrivilege>().AnyAsync(p => p.Name == name);
                return exists;
            }
        }

    }

}
