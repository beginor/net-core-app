using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace Beginor.GisHub.Api.Controllers {

    /// <summary>切片地图 API</summary>
    [ApiController]
    [Route("rest/services/tilemap")]
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
            logger = null;
            repository = null;
        }

        /// <summary>获取全部切片地图名称</summary>
        [HttpGet("")]
        [Authorize("tile_maps.read_tile_list")]
        public IActionResult GetAll() {
            try {
                var tileNames = repository.GetAllTileMapNames();
                return Ok(tileNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Can not get all tile map names!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>读取切片服务信息</summary>
        [HttpGet("{tileName}/MapServer")]
        [Authorize("tile_maps.read_tile_info")]
        public IActionResult GetTileMapInfo(string tileName) {
            try {
                var tileMapInfo  = repository.GetTileMapInfo(tileName);
                var text = tileMapInfo.ToString();
                var hasCallback = Request.Query.TryGetValue("callback", out var callback);
                if (hasCallback) {
                    text = $"{callback.First()}({text})";
                }
                return Content(text, hasCallback ? "text/javascript" : "application/json");
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tilemap info for {tileName}!");
                return this.InternalServerError(ex);
            }
        }

        [HttpGet("{tileName}/MapServer/tile/{level:int}/{row:int}/{col:int}")]
        [Authorize("tile_maps.read_tile")]
        public async Task<IActionResult> GetTile(string tileName, int level, int row, int col) {
            try {
                var modifiedTime = repository.GetTileModifiedTime(tileName, level, row, col);
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
                var content = await repository.GetTileContentAsync(tileName, level, row, col);
                if (content.Content.Length == 0) {
                    return NotFound();
                }
                return File(content.Content, content.ContentType, modifiedTime, new EntityTagHeaderValue(fileEtag));
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get tile {tileName}:{level:int}/{row:int}/{col:int}!");
                return this.InternalServerError(ex.Message);
            }
        }

    }

}
