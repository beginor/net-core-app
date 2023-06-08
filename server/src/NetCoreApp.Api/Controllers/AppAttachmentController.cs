using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>附件表服务接口</summary>
[Route("api/attachments")]
[ApiController]
public class AppAttachmentController : Controller {

    private readonly ILogger<AppAttachmentController> logger;
    private readonly IAppAttachmentRepository repository;
    private readonly UserManager<AppUser> userMgr;

    public AppAttachmentController(
        ILogger<AppAttachmentController> logger,
        IAppAttachmentRepository repository,
        UserManager<AppUser> userMgr
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
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
    public async Task<ActionResult<AppAttachmentModel[]>> Create() {
        var files = Request.Form.Files;
        if (files.Count == 0) {
            return BadRequest("No file in current request ！");
        }
        var businessId = string.Empty;
        const string BusinessId = "businessId";
        if (Request.Query.TryGetValue(BusinessId, out var queryVal)) {
            businessId = queryVal.ToString();
        }
        else if (Request.Form.TryGetValue(BusinessId, out var formVal)) {
            businessId = formVal.ToString();
        }
        if (string.IsNullOrEmpty(businessId)) {
            return BadRequest("businessId is required.");
        }
        if (!long.TryParse(businessId, out _)) {
            return BadRequest("invalid businessId.");
        }

        var models = new List<AppAttachmentModel>();
        try {
            var userId = this.GetUserId();
            var user = await userMgr.FindByIdAsync(userId!);
            foreach (var file in files) {
                var model = new AppAttachmentModel {
                    BusinessId = businessId,
                    ContentType = file.ContentType,
                    FileName = file.FileName,
                    Length = file.Length
                };
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var content = stream.GetBuffer();
                await repository.SaveAsync(model, content, user!);
                models.Add(model);
            }
            return Ok(models);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save to attachments.");
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
    public async Task<ActionResult> GetById(long id, [FromQuery]string? action) {
        try {
            var model = await repository.GetByIdAsync(id);
            if (model == null) {
                return NotFound();
            }
            var content = await repository.GetContentAsync(id);
            var result = new FileContentResult(content, model.ContentType) {
                LastModified = new DateTimeOffset(model.CreatedAt)
            };
            if (action != null && action.EqualsOrdinalIgnoreCase("download")) {
                result.FileDownloadName = model.FileName;
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_attachments by id {id}.");
            return this.InternalServerError(ex);
        }
    }

}
