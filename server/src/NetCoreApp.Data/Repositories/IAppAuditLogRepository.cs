using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>审计日志仓储接口</summary>
public partial interface IAppAuditLogRepository : IRepository<AppAuditLogModel, long> {

    Task<PaginatedResponseModel<AppAuditLogModel>> SearchAsync(
        AppAuditLogSearchModel model
    );

    Task<PaginatedResponseModel<AppAuditLogTrafficStatModel>> StatTrafficAsync(DateTime startDate, DateTime endDate);

    Task<PaginatedResponseModel<AppAuditLogStatusStatModel>> StatStatusAsync(DateTime startDate, DateTime endDate);

    Task<PaginatedResponseModel<AppAuditlogDurationStatModel>> StatsDurationAsync(DateTime startDate, DateTime endDate);

}
