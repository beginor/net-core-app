using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>导航节点（菜单）仓储接口</summary>
    public partial interface IAppNavItemRepository : IRepository<AppNavItem, long> { }

}
