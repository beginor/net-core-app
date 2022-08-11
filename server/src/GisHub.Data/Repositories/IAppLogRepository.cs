using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>应用程序日志 仓储接口</summary>
public partial interface IAppLogRepository : IRepository<AppLogModel, long> {
    /// <summary>搜索 应用程序日志 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<AppLogModel>> SearchAsync(
        AppLogSearchModel model
    );
}
