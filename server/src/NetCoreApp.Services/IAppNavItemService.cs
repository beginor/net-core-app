using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>导航节点（菜单）服务接口</summary>
    public partial interface IAppNavItemService : IBaseService<AppNavItemModel> {

        /// <summary>导航节点（菜单）搜索，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppNavItemModel>> SearchAsync(
            AppNavItemSearchModel model
        );

    }

}
