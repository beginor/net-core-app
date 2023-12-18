using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>组织单元 服务接口</summary>
[ApiController]
[Route("api/organize-units")]
public class AppOrganizeUnitController(
    ILogger<AppOrganizeUnitController> logger,
    IAppOrganizeUnitRepository repository
) : Controller {

    /// <summary>搜索 组织单元 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="400">客户端发送错误请求</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_organize_units.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppOrganizeUnitModel>>> Search(
        [FromQuery]AppOrganizeUnitSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model, User);
            return result;
        }
        catch (InvalidOperationException ex) {
            logger.LogWarning(ex.GetOriginalMessage());
            return BadRequest();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_organize_unit with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary> 创建 组织单元 </summary>
    /// <response code="200">创建 组织单元 成功</response>
    /// <response code="400">客户端发送错误请求</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_organize_units.create")]
    public async Task<ActionResult<AppOrganizeUnitModel>> Create(
        [FromBody]AppOrganizeUnitModel model
    ) {
        try {
            await repository.SaveAsync(model, User);
            return model;
        }
        catch (InvalidOperationException ex) {
            logger.LogWarning(ex.GetOriginalMessage());
            return BadRequest();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to app_organize_units.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 组织单元 </summary>
    /// <response code="204">删除 组织单元 成功</response>
    /// <response code="400">客户端发送错误请求</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_organize_units.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id, User);
            return NoContent();
        }
        catch (InvalidOperationException ex) {
            logger.LogWarning(ex.GetOriginalMessage());
            return BadRequest();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete app_organize_unit by id {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>获取指定的 组织单元</summary>
    /// <response code="200">返回 组织单元 信息</response>
    /// <response code="400">客户端发送错误请求</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_organize_units.read_by_id")]
    public async Task<ActionResult<AppOrganizeUnitModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id, User);
            return result;
        }
        catch (InvalidOperationException ex) {
            logger.LogWarning(ex.GetOriginalMessage());
            return BadRequest();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_organize_unit by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>更新 组织单元</summary>
    /// <response code="200">更新成功，返回 组织单元 信息</response>
    /// <response code="400">客户端发送错误请求</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_organize_units.update")]
    public async Task<ActionResult<AppOrganizeUnitModel>> Update(
        [FromRoute]long id,
        [FromBody]AppOrganizeUnitModel model
    ) {
        try {
            await repository.UpdateAsync(id, model, User);
            return model;
        }
        catch (InvalidOperationException ex) {
            logger.LogWarning(ex.GetOriginalMessage());
            return BadRequest();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update app_organize_unit by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

}
