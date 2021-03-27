using System;

namespace Beginor.GisHub.TileMap.Data {

    public class TileMapCacheItem {
        public string Name { get; set; }
        public string CacheDirectory { get; set; }
        public string MapTileInfoPath { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset? ModifiedTime { get; set; }
        public bool IsBundled { get; set; }

    }

}
