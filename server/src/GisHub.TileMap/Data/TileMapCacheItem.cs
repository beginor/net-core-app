using System;
using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.TileMap.Data {

    public class TileMapCacheItem {
        public string Name { get; set; }
        public string CacheDirectory { get; set; }
        public string MapTileInfoPath { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset? ModifiedTime { get; set; }
        public string FolderStructure { get; set; }
        public bool IsBundled { get; set; }
        public short MinLevel { get; set; }
        public short MaxLevel { get; set; }
        public AgsExtent Extent { get; set; }
    }

}
