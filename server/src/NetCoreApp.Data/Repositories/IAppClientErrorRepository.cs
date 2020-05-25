using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>程序客户端错误记录 仓储接口</summary>
    public partial interface IAppClientErrorRepository : IRepository<AppClientErrorModel, long> {

        /// <summary>搜索 程序客户端错误记录 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppClientErrorModel>> SearchAsync(
            AppClientErrorSearchModel model
        );

    }

}