using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers; 

/// <summary>审计日志统计接口</summary>
[Route("api/audit-log-stats")]
[ApiController]
public class AppAuditLogStatController : Controller {
    
    private ILogger<AppAuditLogStatController> logger;
    private IAppAuditLogRepository repository;
    
    public AppAuditLogStatController(
        ILogger<AppAuditLogStatController> logger,
        IAppAuditLogRepository repository
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            logger = null;
            repository = null;
        }
        base.Dispose(disposing);
    }

    /// <summary>读取访问量统计</summary>
    /// <response code="200">读取访问量统计 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("traffic")]
    // // [Authorize("app_audit_logs.read_stat")]
    public async Task<ActionResult<PaginatedResponseModel<AppAuditLogTrafficStatModel>>> StatTraffic() {
        try {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-29);
            var result = await repository.StatTrafficAsync(startDate, endDate);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not stat traffic from app audit log!");
            return this.InternalServerError(ex);
        }
    }
    
    /// <summary>读取状态码统计</summary>
    /// <response code="200">读取状态码统计 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("status")]
    // [Authorize("app_audit_logs.read_stat")]
    public async Task<ActionResult<PaginatedResponseModel<AppAuditLogStatusStatModel>>> StatStatus() {
        try {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-29);
            var result = await repository.StatStatusAsync(startDate, endDate);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not stat status from app audit log!");
            return this.InternalServerError(ex);
        }
    }
    
    /// <summary>读取响应时间统计</summary>
    /// <response code="200">读取响应时间统计 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("duration")]
    // [Authorize("app_audit_logs.read_stat")]
    public async Task<ActionResult<PaginatedResponseModel<AppAuditLogDurationStatModel>>> StatDuration() {
        try {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-29);
            var result = await repository.StatDurationAsync(startDate, endDate);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not stat duration from app audit log!");
            return this.InternalServerError(ex);
        }
    }
    
    /// <summary>读取用户访问统计</summary>
    /// <response code="200">读取用户访问统计 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("user")]
    // [Authorize("app_audit_logs.read_stat")]
    public async Task<ActionResult<PaginatedResponseModel<AppAuditLogUserStatModel>>> StatUser() {
        try {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-29);
            var result = await repository.StatUserAsync(startDate, endDate);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not stat user request from app audit log!");
            return this.InternalServerError(ex);
        }
    }
    
    /// <summary>读取 ip 地址访问统计</summary>
    /// <response code="200">读取 ip 地址访问统计 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("ip")]
    // [Authorize("app_audit_logs.read_stat")]
    public async Task<ActionResult<PaginatedResponseModel<AppAuditLogIpStatModel>>> StatIp() {
        try {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-29);
            var result = await repository.StatIpAsync(startDate, endDate);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not stat user request from app audit log!");
            return this.InternalServerError(ex);
        }
    }

}
