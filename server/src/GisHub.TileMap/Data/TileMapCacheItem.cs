using System;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.TileMap.Data;

public class TileMapCacheItem {
    public string Name { get; set; } = string.Empty;
    public string CacheDirectory { get; set; } = string.Empty;
    public string MapTileInfoPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTimeOffset? ModifiedTime { get; set; }
    public string FolderStructure { get; set; } = string.Empty;
    public bool IsBundled { get; set; }
    public short MinLevel { get; set; }
    public short MaxLevel { get; set; }
    public AgsExtent? Extent { get; set; }
}
