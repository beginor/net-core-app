using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;

using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>附件服务接口</summary>
[Route("api/attachments")]
[ApiController]
public class AppAttachmentController(
    ILogger<AppAttachmentController> logger,
    IAppAttachmentRepository repository,
    IContentTypeProvider contentTypeProvider,
    AppAttachmentOptions options
) : Controller {

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary> 创建 附件  </summary>
    /// <response code="200">创建 附件 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_attachments.create")]
    public async Task<ActionResult<AttachmentUploadResultModel>> Create(AttachmentUploadModel model) {
        var result = new AttachmentUploadResultModel {
            FileName = model.FileName,
            Length = model.Length
        };
        if (options.MaxSize > 0 && model.Length > options.MaxSize) {
            return BadRequest(/*"附件太大！"*/);
        }
        if (options.MaxBlockSize > 0 && model.Content.Length > options.MaxBlockSize) {
            return BadRequest(/*"附件超过允许的分块大小！"*/);
        }
        if (options.Forbidden.Length > 0) {
            if (options.Forbidden.Any(ext => model.FileName.EndsWith(ext))) {
                return BadRequest(/*$"禁止上传 {Path.GetExtension(model.FileName)} 文件！"*/);
            }
        }
        try {
            var userId = this.GetUserId()!;
            var userTemp = repository.GetAttachmentTempDirectory(userId);
            var tmpPath = Path.Combine(userTemp, model.FileName);
            var fileInfo = new FileInfo(tmpPath);
            await FileHelper.PartialSaveFile(model.Length, fileInfo, model.Offset, model.Content);
            if (model.Length <= model.Offset + model.Content.Length) {
                var contentType = "application/octet-stream";
                if (contentTypeProvider.TryGetContentType(model.FileName, out var ct)) {
                    contentType = ct;
                }
                var attachmentModel = new AppAttachmentModel {
                    Id = "0",
                    FileName = model.FileName,
                    Length = model.Length,
                    ContentType = contentType,
                    CreatedAt = DateTime.Now,
                    CreatorId = userId,
                    BusinessId = model.BusinessId,
                };
                await repository.SaveAsync(attachmentModel, fileInfo, User);
            }
            result.UploadedSize = fileInfo.Length;
            return Ok(result);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not save to attachments.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 附件 </summary>
    /// <response code="204">删除 附件 成功</response>
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

    /// <summary>搜索 附件 ， 分页返回结果</summary>
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
    /// 获取指定的 附件
    /// </summary>
    /// <response code="200">返回 附件 信息</response>
    /// <response code="404"> 附件 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    // [Authorize("app_attachments.read_by_id")]
    public async Task<ActionResult> GetById(long id, [FromQuery]string? action) {
        try {
            var model = await repository.GetByIdAsync(id);
            if (model == null) {
                return NotFound();
            }
            var filePath = Path.Combine(repository.GetAttachmentStorageDirectory(), model.FilePath);
            if (!System.IO.File.Exists(filePath)) {
                logger.LogError($"File {filePath} does not exist, please check storage!");
                return NotFound();
            }
            var result = new PhysicalFileResult(filePath, model.ContentType) {
                LastModified = new DateTimeOffset(model.CreatedAt),
                EnableRangeProcessing = true
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

    /// <summary>
    /// 获取指定的附件的缩略图
    /// </summary>
    /// <response code="200">返回 缩略图 信息</response>
    /// <response code="404"> 缩略图 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}/thumbnail")]
    // [Authorize("app_attachments.read_by_id")]
    public async Task<ActionResult> GetThumbnailById(long id) {
        try {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null) {
                return NotFound();
            }
            var content = await repository.GetThumbnailAsync(id);
            if (content.Length == 0) {
                return NotFound();
            }
            return File(content, entity.ContentType);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get app_attachments thumbnail by id {id}.");
            return this.InternalServerError(ex);
        }
    }

}
