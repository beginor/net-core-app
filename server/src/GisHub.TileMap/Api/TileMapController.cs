using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Data;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Api {

    /// <summary>切片地图 服务接口</summary>
    [ApiController]
    [Route("api/tile-maps")]
    public class TileMapController : Controller {

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
        [Authorize("tile_maps.create")]
        public async Task<ActionResult<TileMapModel>> Create(
            [FromBody]TileMapModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to tile_maps.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 切片地图 </summary>
        /// <response code="204">删除 切片地图 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("tile_maps.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete tile_maps by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 切片地图 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("tile_maps.read")]
        public async Task<ActionResult<PaginatedResponseModel<TileMapModel>>> GetAll(
            [FromQuery]TileMapSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search tile_maps with {model.ToJson()} .");
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
        [Authorize("tile_maps.read")]
        public async Task<ActionResult<TileMapModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tile_maps by id {id}.");
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
        [Authorize("tile_maps.update")]
        public async Task<ActionResult<TileMapModel>> Update(
            [FromRoute]long id,
            [FromBody]TileMapModel model
        ) {
            try {
                var modelInDb = await repository.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await repository.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update tile_maps by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("~/rest/services/tile-maps")]
        [Authorize("tile_maps.read_tile_content")]
        public async Task<ActionResult> GetTileMapList() {
            try {
                var models = await repository.GetAllAsync();
                var tiles = models.Select(m => new { m.Id, m.Name, m.ContentType });
                return Ok(tiles);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tilemap list!");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>读取切片服务信息</summary>
        [HttpGet("~/rest/services/tile-maps/{id}/MapServer")]
        [Authorize("tile_maps.read_tile_content")]
        public async Task<ActionResult> GetTileMapInfo(long id) {
            try {
                var tileMapInfo  = await repository.GetTileMapInfoAsync(id);
                var text = tileMapInfo.ToString();
                var hasCallback = Request.Query.TryGetValue("callback", out var callback);
                if (hasCallback) {
                    text = $"{callback.First()}({text})";
                }
                return Content(text, hasCallback ? "text/javascript" : "application/json");
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tilemap info for {id}!");
                return this.InternalServerError(ex);
            }
        }

        [HttpGet("~/rest/services/tile-maps/{id}/MapServer/tile/{level:int}/{row:int}/{col:int}")]
        [Authorize("tile_maps.read_tile_content")]
        public async Task<IActionResult> GetTile(long id, int level, int row, int col) {
            try {
                var modifiedTime = await repository.GetTileModifiedTimeAsync(id, level, row, col);
                if (!modifiedTime.HasValue) {
                    return NotFound();
                }
                var requestETag = string.Empty;
                if (Request.Headers.TryGetValue("If-None-Match", out var values)) {
                    requestETag = values.FirstOrDefault();
                }
                var fileEtag = modifiedTime.Value.ToUnixTimeMilliseconds().ToString("x");
                if (!string.IsNullOrEmpty(requestETag) && fileEtag.Equals(requestETag, StringComparison.OrdinalIgnoreCase)) {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
                var content = await repository.GetTileContentAsync(id, level, row, col);
                if (content.Content.Length == 0) {
                    return NotFound();
                }
                Response.Headers["Cache-Control"] = "no-cache";
                Response.Headers["ETag"] = fileEtag;
                return File(content.Content, content.ContentType);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tile {id}:{level:int}/{row:int}/{col:int}!");
                return this.InternalServerError(ex.Message);
            }
        }

    }

}
