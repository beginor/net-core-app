using System;
using System.IO;
using System.Threading.Tasks;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap; 

public static class BundleHelper {

    public static DateTimeOffset? GetTileModifiedTime(string tileFolder, int level, int row, int col) {
        var bundlePath = GetTileBundlePath(tileFolder, level, row, col);
        if (!File.Exists(bundlePath)) {
            return null;
        }
        var lastWriteTime = File.GetLastWriteTime(bundlePath);
        var offset = new DateTimeOffset(lastWriteTime);
        return offset;
    }

    public static async Task<TileContentModel> ReadTileContentAsync(string tilePath, int level, int row, int col) {
        var rowGroup = GetGroupIndex(row);
        var colGroup = GetGroupIndex(col);
        // try get from bundle
        // string.Format("{0}\\L{1:D2}\\R{2:X4}C{3:X4}.{4}", tilePath, lev, rowGroup, colGroup, "bundle");
        var bundlePath = GetBundlePath(tilePath, level, rowGroup, colGroup);
        if (string.IsNullOrEmpty(bundlePath) || !File.Exists(bundlePath)) {
            return TileContentModel.Empty;
        }
        var index = 128 * (row - rowGroup) + (col - colGroup);
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
        var content = new TileContentModel();
        //根据切片位置和切片长度获取切片
        content.Content = new byte[length];
        await fs.ReadAsync(content.Content, 0, content.Content.Length);
        fs.Close();
        // content.ContentType = "image/png";
        return content;
    }

    private static int GetGroupIndex(int x) {
        return 128 * (x / 128);
    }

    private static string GetTileBundlePath(string tileFolder, int level, int row, int col) {
        return GetBundlePath(tileFolder, level, GetGroupIndex(row), GetGroupIndex(col));
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

}