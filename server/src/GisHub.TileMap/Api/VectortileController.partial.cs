using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;
using Beginor.GisHub.TileMap.Data;
using Microsoft.AspNetCore.Http;

namespace Beginor.GisHub.TileMap.Api {

    partial class VectortileController {


        [HttpGet("~/rest/services/vectortiles")]
        [Authorize("vectortiles.read_tile_content")]
        public async Task<ActionResult> GetVectorTileList() {
            try {
                var models = await repository.GetAllAsync();
                var tiles = models.Select(m => new { m.Id, m.Name, m.MinZoom, m.MaxZoom });
                return Ok(tiles);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get vectortile list!");
                return this.InternalServerError(ex);
            }
        }

        [HttpGet("~/rest/services/vectortiles/{id:long}/VectorTileServer/tile/{level:int}/{row:int}/{col:int}")]
        [Authorize("vectortiles.read_tile_content")]
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
                Response.Headers["Content-Encoding"] = "gzip";
                return File(content.Content, content.ContentType);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get vector tile {id}:{level:int}/{row:int}/{col:int}!");
                return this.InternalServerError(ex.Message);
            }
        }
    }

}
