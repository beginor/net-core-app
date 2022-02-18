using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.GisHub.Gmap.Data;
using Beginor.GisHub.Gmap.Services;
using Beginor.GisHub.Gmap.Utils;

namespace Beginor.GisHub.Gmap.Api;

[Route("api/wmts")]
public class WmtsController : Controller {

    private YztService service;
    private ILogger<WmtsController> logger;

    public WmtsController(
        YztService service,
        ILogger<WmtsController> logger
    ) {
        this.service = service ?? throw new ArgumentNullException(nameof(service));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            service = null;
            logger = null;
        }
    }

    [HttpGet("{tileName}/{level:int}/{row:int}/{col:int}")]
    public async Task<ActionResult> GetTile(string tileName, int level, int row, int col) {
        try {
            var z = level;
            var extent = new [] {
                new [] { MercatorTileUtil.TileX2Lng(col, z), MercatorTileUtil.TileY2Lat(row, z) },
                new [] { MercatorTileUtil.TileX2Lng(col + 1, z), MercatorTileUtil.TileY2Lat(row + 1, z) }
            };
            // 切片宽度一致， 只需要求切片上下两边的中间点所对应的切片编号即可；
            var halfTileWidth = 360.0 / (1 << z + 1);
            var startX = YztTileUtil.Lng2TileX(extent[0][0] + halfTileWidth, z);
            var startY = YztTileUtil.Lat2TileY(extent[0][1], z);
            var endX = YztTileUtil.Lng2TileX(extent[1][0] - halfTileWidth, z);
            var endY = YztTileUtil.Lat2TileY(extent[1][1], z);
            var tiles = new List<Tile>();
            for (var y = startY; y <= endY; y++) {
                for (var x = startX; x <= endX; x++) {
                    tiles.Add(new Tile(x, y, z));
                }
            }
            // 从粤政图服务获取切片内容
            foreach (var tile in tiles) {
                await service.GetTileContent(tileName, tile);
            }
            // 切片内容合成一幅图片
            var result = YztTileUtil.CropTiles(tiles, extent);
            if (result == null) {
                return NotFound();
            }
            return File(result, "image/png");
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get tile for {tileName}/{level}/{row}/{col}");
            return NotFound();
        }

    }

}
