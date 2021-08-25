using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>用户访问凭证 仓储接口</summary>
    public partial interface IUserAccessTokenRepository : IRepository<UserAccessTokenModel, long> {

        /// <summary>搜索 用户访问凭证 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<UserAccessTokenModel>> SearchAsync(
            UserAccessTokenSearchModel model
        );

    }

}