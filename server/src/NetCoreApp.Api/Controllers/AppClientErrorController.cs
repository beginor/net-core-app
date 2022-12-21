using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>程序客户端错误记录 服务接口</summary>
[ApiController]
[Route("api/client-errors")]
public class AppClientErrorController : Controller {

    private ILogger<AppClientErrorController> logger;
    private IAppClientErrorRepository repository;

    public AppClientErrorController(
        ILogger<AppClientErrorController> logger,
        IAppClientErrorRepository repository
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary> 创建 程序客户端错误记录 </summary>
    /// <response code="200">创建 程序客户端错误记录 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_client_errors.create")]
    public async Task<ActionResult<AppClientErrorModel>> Create(
        [FromBody]AppClientErrorModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to app_client_errors.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 程序客户端错误记录 </summary>
    /// <response code="204">删除 程序客户端错误记录 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_client_errors.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete app_client_errors by id {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>搜索 程序客户端错误记录 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_client_errors.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppClientErrorModel>>> GetAll(
        [FromQuery]AppClientErrorSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_client_errors with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 程序客户端错误记录
    /// </summary>
    /// <response code="200">返回 程序客户端错误记录 信息</response>
    /// <response code="404"> 程序客户端错误记录 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_client_errors.read_by_id")]
    public async Task<ActionResult<AppClientErrorModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_client_errors by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 更新 程序客户端错误记录
    /// </summary>
    /// <response code="200">更新成功，返回 程序客户端错误记录 信息</response>
    /// <response code="404"> 程序客户端错误记录 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_client_errors.update")]
    public async Task<ActionResult<AppClientErrorModel>> Update(
        [FromRoute]long id,
        [FromBody]AppClientErrorModel model
    ) {
        try {
            var exists = await repository.ExistAsync(id);
            if (!exists) {
                return NotFound();
            }
            await repository.UpdateAsync(id, model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update app_client_errors by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

}
