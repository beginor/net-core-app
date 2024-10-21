using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>json 数据 服务接口</summary>
[ApiController]
[Route("api/json")]
public class AppJsonDataController : Controller {

    private ILogger<AppJsonDataController> logger;
    private IAppJsonDataRepository repository;

    public AppJsonDataController(
        ILogger<AppJsonDataController> logger,
        IAppJsonDataRepository repository
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

    /// <summary>搜索 Json 数据 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_json_data.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppJsonDataModel>>> Search(
        [FromQuery]AppJsonDataSearchModel model
    ) {
        var businessId = model.BusinessId;
        var name = model.Name;
        // 必须提供 businessId 或者 Name 参数， 否则不允许查询；
        if (businessId == 0L && string.IsNullOrEmpty(name)) {
            return BadRequest();
        }
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_json_data with {model.ToJson()}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary> 创建 Json 数据  </summary>
    /// <response code="200">创建 Json 数据 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_json_data.create")]
    public async Task<ActionResult<AppJsonDataModel>> Create(
        [FromBody]AppJsonDataModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to app_json_data.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 json 数据 </summary>
    /// <response code="204">删除 json 数据 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_json_data.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete app_json_data by id {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 json 数据
    /// </summary>
    /// <response code="200">返回 json 数据 信息</response>
    /// <response code="404"> json 数据 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_json_data.read_by_id")]
    public async Task<ActionResult> GetById(long id) {
        try {
            var model = await repository.GetByIdAsync(id);
            if (model == null || model.Value.ValueKind == JsonValueKind.Undefined) {
                return NotFound();
            }
            return Ok(model.Value);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_json_data by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 更新 json 数据
    /// </summary>
    /// <response code="200">更新成功，返回 json 数据 信息</response>
    /// <response code="404"> json 数据 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_json_data.update")]
    public async Task<ActionResult> Update(
        [FromRoute]long id,
        [FromBody]AppJsonDataModel model
    ) {
        try {
            var exists = await repository.ExistAsync(id);
            if (!exists) {
                return NotFound();
            }
            await repository.UpdateAsync(id, model);
            return Ok();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update app_json_data by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

}
