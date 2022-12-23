using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Api.Controllers;

/// <summary>附件表服务接口</summary>
[Route("api/attachments")]
[ApiController]
public class AppAttachmentController : Controller {

    private ILogger<AppAttachmentController> logger;
    private IAppAttachmentRepository repository;

    public AppAttachmentController(
        ILogger<AppAttachmentController> logger,
        IAppAttachmentRepository repository
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

    /// <summary> 创建 附件表  </summary>
    /// <response code="200">创建 附件表 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_attachments.create")]
    public async Task<ActionResult<AppAttachmentModel>> Create(
        [FromBody]AppAttachmentModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to app_attachments.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 附件表 </summary>
    /// <response code="204">删除 附件表 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_attachments.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete app_attachments by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>搜索 附件表 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("app_attachments.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppAttachmentModel>>> GetAll(
        [FromQuery]AppAttachmentSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search app_attachments with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 附件表
    /// </summary>
    /// <response code="200">返回 附件表 信息</response>
    /// <response code="404"> 附件表 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_attachments.read_by_id")]
    public async Task<ActionResult<AppAttachmentModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_attachments by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 更新 附件表
    /// </summary>
    /// <response code="200">更新成功，返回 附件表 信息</response>
    /// <response code="404"> 附件表 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_attachments.update")]
    public async Task<ActionResult<AppAttachmentModel>> Update(
        [FromRoute]long id,
        [FromBody]AppAttachmentModel model
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
            logger.LogError(ex, $"Can not update app_attachments by {id} with {model.ToJson()}.");
            return this.InternalServerError(ex);
        }
    }

}
