using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.TileMap.Api; 

partial class TileMapController {

    /// <summary>读取切片服务列表</summary>
    [HttpGet("~/rest/services/tilemaps")]
    [Authorize("tilemaps.read_tile_content")]
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
    [HttpGet("~/rest/services/tilemaps/{id:long}/MapServer")]
    [Authorize("tilemaps.read_tile_content")]
    public async Task<ActionResult> GetTileMapInfo(long id) {
        try {
            var tileMapInfo = await repository.GetTileMapInfoAsync(id);
            var text = tileMapInfo.ToString();
            var hasCallback = Request.Query.TryGetValue("callback", out var callback);
            if (hasCallback) {
                text = $"{callback.First()}({text})";
            }
            return this.CompressedContent(text, hasCallback ? "text/javascript" : "application/json");
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get tilemap info for {id}!");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>读取切片服务的切片</summary>
    [HttpGet("~/rest/services/tilemaps/{id:long}/MapServer/tile/{level:int}/{row:int}/{col:int}")]
    [Authorize("tilemaps.read_tile_content")]
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
            Response.Headers.CacheControl = "no-cache";
            Response.Headers.ETag = fileEtag;
            if (!content.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) {
                Response.Headers.ContentEncoding = "gzip";
            }
            return File(content.Content, content.ContentType);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get tile {id}:{level:int}/{row:int}/{col:int}!");
            return this.InternalServerError(ex.Message);
        }
    }

}