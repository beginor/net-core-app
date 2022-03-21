using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Api.Controllers; 

/// <summary>系统权限服务接口</summary>
[Route("api/privileges")]
[ApiController]
public class AppPrivilegeController : Controller {

    private ILogger<AppPrivilegeController> logger;
    private IAppPrivilegeRepository repository;

    public AppPrivilegeController(
        ILogger<AppPrivilegeController> logger,
        IAppPrivilegeRepository repository
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

    /// <summary> 创建 系统权限  </summary>
    /// <response code="200">创建 系统权限 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_privileges.create")]
    public async Task<ActionResult<AppPrivilegeModel>> Create(
        [FromBody]AppPrivilegeModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to app_privileges.", ex);
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 系统权限 </summary>
    /// <response code="204">删除 系统权限 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_privileges.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete app_privileges by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>搜索 系统权限 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_privileges.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppPrivilegeModel>>> GetAll(
        [FromQuery]AppPrivilegeSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_privileges with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 系统权限
    /// </summary>
    /// <response code="200">返回 系统权限 信息</response>
    /// <response code="404"> 系统权限 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_privileges.read_by_id")]
    public async Task<ActionResult<AppPrivilegeModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_privileges by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 更新 系统权限
    /// </summary>
    /// <response code="200">更新成功，返回 系统权限 信息</response>
    /// <response code="404"> 系统权限 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_privileges.update")]
    public async Task<ActionResult<AppPrivilegeModel>> Update(
        [FromRoute]long id,
        [FromBody]AppPrivilegeModel model
    ) {
        try {
            var exists = await repository.ExistAsync(id);
            if (!exists) {
                return NotFound();
            }
            model.Id = id.ToString();
            await repository.UpdateAsync(id, model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update app_privileges by {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>同步系统必须的权限</summary>
    /// <response code="200">同步成功， 客户端刷新权限列表即可看到更新。</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("sync-required")]
    [Authorize("app_privileges.sync_required")]
    public async Task<ActionResult> SyncRequired() {
        try {
            var assembly = GetType().Assembly;
            var policies = assembly.ExportedTypes
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                .SelectMany(m => m.GetCustomAttributes<AuthorizeAttribute>(false))
                .Select(attr => attr.Policy);
            await repository.SyncRequiredAsync(policies);
            return Ok();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not sync required app_privileges.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>获取权限表的模块列表</summary>
    /// <response code="200">获取成功，返回模块列表。</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("~/api/modules")]
    [Authorize("app_privileges.read_modules")]
    public async Task<ActionResult<string[]>> GetModules() {
        try {
            var modules = await repository.GetModulesAsync();
            return modules.ToArray();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get all modules.");
            return this.InternalServerError(ex);
        }
    }

}