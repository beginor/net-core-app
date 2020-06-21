using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.TileMap {

    public class TileMapRepository: Disposable, ITileMapRepository {

        private ILogger<TileMapRepository> logger;
        private TileMapOptions options;

        public TileMapRepository(ILogger<TileMapRepository> logger, TileMapOptions options) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                options = null;
            }
        }

        public IList<string> GetAllTileMapNames() {
            var folderInfo = new DirectoryInfo(options.CacheFolder);
            var subDirs = folderInfo.EnumerateDirectories()
                .Where(dir => !dir.Name.StartsWith(".", StringComparison.Ordinal))
                .Select(dir => dir.Name)
                .ToList();
            return subDirs;
        }

        public async Task<TileContent> GetTileContentAsync(string tileName, int level, int row, int col) {
            var tilePath = GetTilePath(tileName);
            if (tilePath.IsNullOrEmpty()) {
                return TileContent.Empty;
            }
            var content = await ReadTileContentAsync(tilePath, level, row, col);
            return content;
        }

        public JsonElement GetTileMapInfo(string tileName) {
            Argument.NotNullOrEmpty(tileName, nameof(tileName));
            var text = File.ReadAllText(options.MapInfoTemplateFile)
                .Replace("#name#", tileName)
                .Replace("#description#", $"{tileName} Tile Server")
                .Replace("#copyright#", $"{tileName} Tile Server by GisHub");
            return JsonDocument.Parse(text).RootElement;
        }

        public DateTimeOffset? GetTileModifiedTime(string tileName, int level, int row, int col) {
            var tilePath = GetTilePath(tileName);
            if (tilePath.IsNullOrEmpty()) {
                return null;
            }
            var rowGroup = GetGroupIndex(row);
            var colGroup = GetGroupIndex(col);
            var bundlePath = GetBundlePath(tilePath, level, rowGroup, colGroup);
            if (!File.Exists(bundlePath)) {
                return null;
            }
            var lastWriteTime = File.GetLastWriteTime(bundlePath);
            return new DateTimeOffset(lastWriteTime);
        }

        private string GetTilePath(string tileName) {
            var tilePath = Path.Combine(options.CacheFolder, tileName, "Layers", "_alllayers");
            if (Directory.Exists(tilePath)) {
                return tilePath;
            }
            tilePath = Path.Combine(options.CacheFolder, "BaseMap_" + tileName, "Layers", "_alllayers");
            if (Directory.Exists(tilePath)) {
                return tilePath;
            }
            return string.Empty;
        }

        private static async Task<TileContent> ReadTileContentAsync(string tilePath, int level, int row, int col) {
            var bundleContent = await ReadTileContentFromBundleAsync(tilePath, level, row, col);
            if (bundleContent.Content.Length > 0) {
                return bundleContent;
            }
            var fileContent = await ReadTileContentFromFileAsync(tilePath, level, row, col);
            if (fileContent.Content.Length > 0) {
                return bundleContent;
            }
            return TileContent.Empty;
        }

        private static async Task<TileContent> ReadTileContentFromFileAsync(string tilePath, int level, int row, int col) {
            // var tileFileName = string.Format("{0}\\L{1:D2}\\R{2:X8}\\C{3:X8}", tilePath, lev, r, c);
            var filePath = Path.Combine(tilePath, level.ToString("D2"), row.ToString("X8"), col.ToString("X8"));
            if (File.Exists(filePath + ".png")) {
                filePath = filePath + ".png";
            }
            else if (File.Exists(filePath + ".jpg")) {
                filePath = filePath + ".jpg";
            }
            else {
                return TileContent.Empty;
            }
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var length = (int)fs.Length;
            var buffer = new byte[length];
            await fs.ReadAsync(buffer, 0, length);
            fs.Close();
            return new TileContent {
                Content = buffer,
                ContentType = filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg"
            };
        }
        
        private static async Task<TileContent> ReadTileContentFromBundleAsync(string tilePath, int level, int row, int col) {
            var content = new TileContent();
            var rowGroup = GetGroupIndex(row);
            var colGroup = GetGroupIndex(col);
            // try get from bundle
            // string.Format("{0}\\L{1:D2}\\R{2:X4}C{3:X4}.{4}", tilePath, lev, rowGroup, colGroup, "bundle");
            var bundlePath = GetBundlePath(tilePath, level, rowGroup, colGroup);
            var index = 128 * (row - rowGroup) + (col - colGroup);
            if (string.IsNullOrEmpty(bundlePath) || !File.Exists(bundlePath)) {
                return TileContent.Empty;
            }
            using var fs = new FileStream(bundlePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            fs.Seek(64 + 8 * index, SeekOrigin.Begin);
            // 获取位置索引并计算切片位置偏移量
            var indexBytes = new byte[4];
            await fs.ReadAsync(indexBytes, 0, 4);
            var offset = (indexBytes[0] & 0xff)
                + (long)(indexBytes[1] & 0xff) * 256
                + (long)(indexBytes[2] & 0xff) * 65536
                + (long)(indexBytes[3] & 0xff) * 16777216;
            // 获取切片长度索引并计算切片长度
            var startOffset = offset - 4;
            fs.Seek(startOffset, SeekOrigin.Begin);
            var lengthBytes = new byte[4];
            await fs.ReadAsync(lengthBytes, 0, 4);
            var length = (lengthBytes[0] & 0xff)
                + (lengthBytes[1] & 0xff) * 256
                + (lengthBytes[2] & 0xff) * 65536
                + (lengthBytes[3] & 0xff) * 16777216;
            //根据切片位置和切片长度获取切片
            content.Content = new byte[length];
            await fs.ReadAsync(content.Content, 0, content.Content.Length);
            fs.Close();
            content.ContentType = "image/png";
            return content;
        }

        private static string GetBundlePath(string tileFolder, int level, int rowGroup, int colGroup) {
            var bundlePath = Path.Combine(tileFolder, $"L{level:D2}", $"R{rowGroup:X4}C{colGroup:X4}.bundle");
            if (File.Exists(bundlePath)) {
                return bundlePath;
            }
            bundlePath = Path.Combine(tileFolder, $"L{level:D2}", $"R{rowGroup:x4}C{colGroup:x4}.bundle");
            if (File.Exists(bundlePath)) {
                return bundlePath;
            }
            return string.Empty;
        }

        private static int GetGroupIndex(int x) {
            return 128 * (x / 128);
        }

    }

}
