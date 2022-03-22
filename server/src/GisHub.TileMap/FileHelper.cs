using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap; 

public static class FileHelper {

    private static Dictionary<string, string> ContentTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg"
    };

    public static DateTimeOffset? GetTileModifiedTime(string tileFolder, int level, int row, int col, string folderStructure = "esri") {
        var filePath = folderStructure == "esri" ? GetEsriTilePath(tileFolder, level, row, col) : GetGdalTilePath(tileFolder, level, row, col);
        if (filePath.IsNullOrEmpty()) {
            return null;
        }
        if (!File.Exists(filePath)) {
            return null;
        }
        var lastWriteTime = File.GetLastWriteTime(filePath);
        var offset = new DateTimeOffset(lastWriteTime);
        return offset;
    }

    public static async Task<TileContentModel> ReadTileContentAsync(string tileFolder, int level, int row, int col, string folderStructure = "esri") {
        var filePath = folderStructure == "esri" ? GetEsriTilePath(tileFolder, level, row, col) : GetGdalTilePath(tileFolder, level, row, col);
        if (filePath.IsNullOrEmpty()) {
            return TileContentModel.Empty;
        }
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var length = (int)fs.Length;
        var buffer = new byte[length];
        await fs.ReadAsync(buffer, 0, length);
        fs.Close();
        var tileContent = new TileContentModel {
            Content = buffer,
            ContentType = ContentTypeMap.GetValueOrDefault(Path.GetExtension(filePath))
        };
        return tileContent;
    }

    private static string GetEsriTilePath(string tileFolder, int level, int row, int col) {
        var tilePath = Path.Combine(tileFolder, level.ToString("D2"), row.ToString("X8"), col.ToString("X8"));
        foreach (var ext in ContentTypeMap.Keys) {
            var path = tilePath + ext;
            if (File.Exists(path)) {
                return path;
            }
        }
        return string.Empty;
    }

    private static string GetGdalTilePath(string tileFolder, int level, int row, int col) {
        var tilePath = Path.Combine(tileFolder, level.ToString(), col.ToString(), row.ToString());
        foreach (var ext in ContentTypeMap.Keys) {
            var path = tilePath + ext;
            if (File.Exists(path)) {
                return path;
            }
        }
        return string.Empty;
    }

}