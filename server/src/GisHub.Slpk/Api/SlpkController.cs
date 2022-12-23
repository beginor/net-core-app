﻿using System;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.AppFx.Api;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Slpk.Data;
using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Slpk.Api;

[Route("api/slpks")]
[ApiController]
public partial class SlpkController : Controller {

    private ILogger<SlpkController> logger;
    private ISlpkRepository repository;
    private IContentTypeProvider provider;
    private UserManager<AppUser> userMgr;

    public SlpkController(
        ILogger<SlpkController> logger,
        ISlpkRepository repository,
        IContentTypeProvider provider,
        UserManager<AppUser> userMgr
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary> 创建 slpk 航拍模型 </summary>
    /// <response code="200">创建 slpk 航拍模型 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("slpks.create")]
    public async Task<ActionResult<SlpkModel>> Create(
        [FromBody]SlpkModel model
    ) {
        try {
            var userId = this.GetUserId();
            var user = await userMgr.FindByIdAsync(userId!);
            await repository.SaveAsync(model, user!);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to slpks.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>删除 slpk 航拍模型 </summary>
    /// <response code="204">删除 slpk 航拍模型 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("slpks.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            var userId = this.GetUserId();
            var user = await userMgr.FindByIdAsync(userId!);
            await repository.DeleteAsync(id, user!);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete slpks by id {id} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>搜索 slpk 航拍模型 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("slpks.read")]
    public async Task<ActionResult<PaginatedResponseModel<SlpkModel>>> Search(
        [FromQuery]SlpkSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search slpks with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 获取指定的 slpk 航拍模型
    /// </summary>
    /// <response code="200">返回 slpk 航拍模型 信息</response>
    /// <response code="404"> slpk 航拍模型 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("slpks.read_by_id")]
    public async Task<ActionResult<SlpkModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get slpks by id {id}.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 更新 slpk 航拍模型
    /// </summary>
    /// <response code="200">更新成功，返回 slpk 航拍模型 信息</response>
    /// <response code="404"> slpk 航拍模型 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("slpks.update")]
    public async Task<ActionResult<SlpkModel>> Update(
        [FromRoute]long id,
        [FromBody]SlpkModel model
    ) {
        try {
            var modelInDb = await repository.GetByIdAsync(id);
            if (modelInDb == null) {
                return NotFound();
            }
            var userId = this.GetUserId();
            var user = await userMgr.FindByIdAsync(userId!);
            await repository.UpdateAsync(id, model, user!);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update slpks by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

}
