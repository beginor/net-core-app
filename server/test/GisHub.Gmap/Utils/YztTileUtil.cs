using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Gmap.Data;

namespace Gmap.Utils {

    /// <summary>粤政图切片工具类</summary>
    public static class YztTileUtil {

        const int TileSize = 256;

        public static int Lng2TileX(double lng, int z) {
            return (int)(Math.Floor((lng + 180.0) / 360.0 * (1 << z)));
        }

        public static int Lat2TileY(double lat, int z) {
            return (int)(Math.Floor((90.0 - lat) / 360 * (1 << z)));
        }

        public static double TileX2Lng(int x, int z) {
            return x / (double)(1 << z) * 360.0 - 180.0;
        }

        public static double TileY2Lat(int y, int z) {
            return 90.0 - y / (double)(1 << z) * 360.0;
        }

        public static byte[] CropTiles(IList<Tile> tiles, double[][] extent) {
            if (tiles.Count == 1) {
                return CropTile(tiles[0], extent);
            }
            var startTile = tiles[0];
            var endTile = tiles[^1];
            var startX = startTile.X;
            var startY = startTile.Y;
            var endX = endTile.X;
            var endY = endTile.Y;
            var width = (endX - startX + 1) * TileSize;
            var height = (endY - startY + 1) * TileSize;
            var rect = ComputedCropedRange(startX, startY, endX, endY, startTile.Z, extent);
            using var image = new Image<Rgba32>(width, height);
            image.Mutate(ctx => {
                foreach (var tile in tiles) {
                    if (tile.IsEmpty) {
                        continue;
                    }
                    var tileImg = Image.Load(tile.Content);
                    var location = new Point((tile.X - startX) * 256, (tile.Y - startY) * 256);
                    ctx.DrawImage(tileImg, location, 1);
                }
                ctx.Crop(rect);
                ctx.Resize(TileSize, TileSize);
            });
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Flush();
            return stream.GetBuffer();
        }

        public static byte[] CropTile(Tile tile, double[][] extent) {
            if (tile.IsEmpty) {
                return null;
            }
            using var image = Image.Load(tile.Content);
            var rect = ComputedCropedRange(tile.X, tile.Y, tile.X, tile.Y, tile.Z, extent);
            image.Mutate(ctx => {
                ctx.Crop(rect);
                ctx.Resize(TileSize, TileSize);
            });
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            return stream.GetBuffer();
        }

        // 根据切片范围以及目标经纬度范围计算要裁切的区域
        private static Rectangle ComputedCropedRange(int startX, int startY, int endX, int endY, int z, double[][] extent) {
            // 切片左上角经纬度
            var startLat = TileY2Lat(startY, z);
            var startLng = TileX2Lng(startX, z);
            // 切片右下角经纬度
            var endLat = TileY2Lat(endY + 1, z);
            var endLng = TileX2Lng(endX + 1, z);
            var imgWidth = (endX - startX + 1) * TileSize;
            var imgHeight = (endY - startY + 1) * TileSize;
            // 开始裁切的位置
            var y = (extent[0][1] - startLat) / (endLat - startLat) * imgHeight;
            var x = (extent[0][0] - startLng) / (endLng - startLng) * imgWidth;
            var width = (extent[1][0] - extent[0][0]) / (endLng - startLng) * imgWidth;
            if (width > TileSize) {
                width = TileSize;
            }
            var height = (extent[1][1] - extent[0][1]) / (endLat - startLat) * imgHeight;
            if (height > TileSize) {
                height = TileSize;
            }
            var rect = new Rectangle((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Ceiling(width), (int)Math.Ceiling(height));
            return rect;
        }

    }

}
