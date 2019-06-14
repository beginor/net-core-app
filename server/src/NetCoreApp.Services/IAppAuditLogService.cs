using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>审计日志服务接口</summary>
    public partial interface IAppAuditLogService : IBaseService<AppAuditLogModel> {

        /// <summary>审计日志搜索，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
            AppAuditLogSearchModel model
        );

    }

}
