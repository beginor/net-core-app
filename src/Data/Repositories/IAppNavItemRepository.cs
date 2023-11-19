using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>导航节点（菜单）仓储接口</summary>
public partial interface IAppNavItemRepository : IRepository<AppNavItemModel, long> {

    Task<PaginatedResponseModel<AppNavItemModel>> SearchAsync(
        AppNavItemSearchModel model
    );

    Task SaveAsync(AppNavItemModel model, string userName);

    Task UpdateAsync(long id, AppNavItemModel model, string userName);

    Task<MenuNodeModel> GetMenuAsync(string[] roles);

}
