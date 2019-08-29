using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>系统权限仓储接口</summary>
    public partial interface IAppPrivilegeRepository : IRepository<AppPrivilege, long> {

        /// <summary>是否存在指定名称的权限</summary>
        Task<bool> ExistsAsync(string name);

    }

}
