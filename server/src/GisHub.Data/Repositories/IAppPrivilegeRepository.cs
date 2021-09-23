using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>系统权限仓储接口</summary>
    public partial interface IAppPrivilegeRepository : IRepository<AppPrivilegeModel, long> {

        /// <summary>返回权限表的所有模块</summary>
        Task<IList<string>> GetModulesAsync();

        /// <summary>系统权限搜索，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppPrivilegeModel>> SearchAsync(
            AppPrivilegeSearchModel model
        );

        /// <summary>同步必须的权限</summary>
        Task SyncRequiredAsync(IEnumerable<string> names);

        Task<IList<AppPrivilegeModel>> GetByNamesAsync(IList<string> names);

    }

}
