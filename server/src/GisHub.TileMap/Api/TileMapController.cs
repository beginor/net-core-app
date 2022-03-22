using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Data;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Api; 

/// <summary>切片地图 服务接口</summary>
[ApiController]
[Route("api/tilemaps")]
public partial class TileMapController : Controller {

    private ILogger<TileMapController> logger;
    private ITileMapRepository repository;

    public TileMapController(
        ILogger<TileMapController> logger,
        ITileMapRepository repository
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

    /// <summary> 创建 切片地图 </summary>
    /// <response code="200">创建 切片地图 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("tilemaps.create")]
    public async Task<ActionResult<TileMapModel>> Create(
        [FromBody]TileMapModel model
    ) {
        try {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await repository.SaveAsync(model, userId);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save {model.ToJson()} to tilemaps.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>删除 切片地图 </summary>
    /// <response code="204">删除 切片地图 成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("tilemaps.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await repository.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete tilemaps by id {id} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>搜索 切片地图 ， 分页返回结果</summary>
    /// <response code="200">成功, 分页返回结果</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [Authorize("tilemaps.read")]
    public async Task<ActionResult<PaginatedResponseModel<TileMapModel>>> Search(
        [FromQuery]TileMapSearchModel model
    ) {
        try {
            var result = await repository.SearchAsync(model);
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search tilemaps with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 获取指定的 切片地图
    /// </summary>
    /// <response code="200">返回 切片地图 信息</response>
    /// <response code="404"> 切片地图 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("tilemaps.read_by_id")]
    public async Task<ActionResult<TileMapModel>> GetById(long id) {
        try {
            var result = await repository.GetByIdAsync(id);
            if (result == null) {
                return NotFound();
            }
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get tilemaps by id {id}.");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

    /// <summary>
    /// 更新 切片地图
    /// </summary>
    /// <response code="200">更新成功，返回 切片地图 信息</response>
    /// <response code="404"> 切片地图 不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("tilemaps.update")]
    public async Task<ActionResult<TileMapModel>> Update(
        [FromRoute]long id,
        [FromBody]TileMapModel model
    ) {
        try {
            var modelInDb = await repository.GetByIdAsync(id);
            if (modelInDb == null) {
                return NotFound();
            }
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await repository.UpdateAsync(id, model, userId);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update tilemaps by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex.GetOriginalMessage());
        }
    }

}