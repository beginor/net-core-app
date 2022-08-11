using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers;

/// <summary>应用程序日志 服务接口</summary>
[ApiController]
[Route("api/logs")]
public class AppLogController : Controller {

    private ILogger<AppLogController> logger;
    private IAppLogRepository repository;

    public AppLogController(
        ILogger<AppLogController> logger,
        IAppLogRepository repository
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
    /// <summary>搜索 应用程序日志 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_logs.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppLogModel>>> Search(
        [FromQuery]AppLogSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_logs with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 应用程序日志
    /// </summary>
    /// <response code="200">返回 应用程序日志 信息</response>
    /// <response code="404"> 应用程序日志 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_logs.read_by_id")]
    public async Task<ActionResult<AppLogModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_logs by id {id}.");
            return this.InternalServerError(ex);
        }
    }

}
