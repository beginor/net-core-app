using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>系统权限服务接口</summary>
    public partial interface IAppPrivilegeService : IBaseService<AppPrivilegeModel> {

        /// <summary>系统权限搜索，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppPrivilegeModel>> SearchAsync(
            AppPrivilegeSearchModel model
        );

        /// <summary>同步必须的权限</summary>
        Task SyncRequired(IEnumerable<string> names);

    }

}
