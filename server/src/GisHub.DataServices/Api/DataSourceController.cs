using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api;

/// <summary>数据源 服务</summary>
[ApiController]
[Route("api/datasources")]
public class DataSourceController : Controller {

    private ILogger<DataSourceController> logger;
    private IDataSourceRepository repository;
    private IDataServiceFactory factory;

    public DataSourceController(
        ILogger<DataSourceController> logger,
        IDataSourceRepository repository,
        IDataServiceFactory factory
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>获取全部的数据源列表</summary>
    [HttpGet("~/api/datasources-list")]
    [Authorize("data_sources.read")]
    public async Task<ActionResult<List<DataSourceModel>>> GetAllForDisplayAsync() {
        try {
            var result = await repository.GetAllForDisplayAsync();
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Can not get connection-strings-list .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>搜索 数据源 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("data_sources.read")]
    public async Task<ActionResult<PaginatedResponseModel<DataSourceModel>>> Search(
        [FromQuery]DataSourceSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search datasource with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary> 创建 数据源 </summary>
    /// <response code="200">创建 数据源 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("data_sources.create")]
    public async Task<ActionResult<DataSourceModel>> Create(
        [FromBody]DataSourceModel model
    ) {
        try {
            await repository.SaveAsync(model);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to datasources.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>删除 数据源 </summary>
    /// <response code="204">删除 数据源 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("data_sources.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            await repository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete datasource by id {id} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 获取指定的 数据源
    /// </summary>
    /// <response code="200">返回 数据源 信息</response>
    /// <response code="404"> 数据源 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("data_sources.read_by_id")]
    public async Task<ActionResult<DataSourceModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get datasource by id {id}.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 更新 数据源
    /// </summary>
    /// <response code="200">更新成功，返回 数据源 信息</response>
    /// <response code="404"> 数据源 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("data_sources.update")]
    public async Task<ActionResult<DataSourceModel>> Update(
        [FromRoute]long id,
        [FromBody]DataSourceModel model
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
            logger.LogError(ex, $"Can not update datasource by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    [HttpPost("~/api/datasources-status")]
    [Authorize("data_sources.read")]
    public async Task<ActionResult> CheckStatus(
        [FromBody]DataSourceModel model
    ) {
        try {
            var metadataProvider = factory.CreateMetadataProvider(model.DatabaseType);
            if (metadataProvider == null) {
                return this.InternalServerError($"Unsupported database type {model.DatabaseType}");
            }
            await metadataProvider.GetStatusAsync(model);
            try {
                await metadataProvider.GetTablesAsync(model, string.Empty);
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tables for datasource {model.ToJson()}");
                return StatusCode(StatusCodes.Status400BadRequest, ex.GetOriginalMessage());
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not check status for datasource {model.ToJson()}");
            return StatusCode(StatusCodes.Status400BadRequest, ex.GetOriginalMessage());
        }
    }

}
