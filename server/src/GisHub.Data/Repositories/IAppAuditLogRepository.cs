using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories; 

/// <summary>审计日志仓储接口</summary>
public partial interface IAppAuditLogRepository : IRepository<AppAuditLogModel, long> {

    Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
        AppAuditLogSearchModel model
    );

}