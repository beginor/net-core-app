using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>审计日志仓储接口</summary>
    public partial interface IAppAuditLogRepository : IRepository<AppAuditLog, long> { }

}
