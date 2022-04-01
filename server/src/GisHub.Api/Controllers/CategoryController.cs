using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers;

/// <summary>数据类别 服务接口</summary>
[ApiController]
[Route("api/categories")]
public class CategoryController : Controller {

    private ILogger<CategoryController> logger;
    private ICategoryRepository repository;

    public CategoryController(
        ILogger<CategoryController> logger,
        ICategoryRepository repository
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

    /// <summary>获取全部 数据类别 。</summary>
    /// <response code="200">成功, 返回全部数据类别</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("categories.read")]
    public async Task<ActionResult<List<CategoryModel>>> GetAll(
    ) {
        try {
            var result = await repository.GetAllAsync();
            return result.ToList();
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not get all categories .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary> 创建 数据类别 </summary>
    /// <response code="200">创建 数据类别 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("categories.create")]
    public async Task<ActionResult<CategoryModel>> Create(
        [FromBody]CategoryModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to categories.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除 数据类别 </summary>
    /// <response code="204">删除 数据类别 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("categories.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete categories by id {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 获取指定的 数据类别
    /// </summary>
    /// <response code="200">返回 数据类别 信息</response>
    /// <response code="404"> 数据类别 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("categories.read_by_id")]
    public async Task<ActionResult<CategoryModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get categories by id {id}.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    /// 更新 数据类别
    /// </summary>
    /// <response code="200">更新成功，返回 数据类别 信息</response>
    /// <response code="404"> 数据类别 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("categories.update")]
    public async Task<ActionResult<CategoryModel>> Update(
        [FromRoute]long id,
        [FromBody]CategoryModel model
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
            logger.LogError(ex, $"Can not update categories by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

}
