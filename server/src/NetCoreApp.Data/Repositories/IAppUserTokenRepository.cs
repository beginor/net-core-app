using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>用户凭证 仓储接口</summary>
    public partial interface IAppUserTokenRepository : IRepository<AppUserTokenModel, long> {

        /// <summary>搜索 用户凭证 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppUserTokenModel>> SearchAsync(
            AppUserTokenSearchModel model
        );

    }

}