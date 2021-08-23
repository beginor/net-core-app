using Beginor.GisHub.Geo.Esri;

namespace Beginor.GisHub.TileMap.Data {

    public static class EntityExtensions {

        public static AgsExtent GetExtent(this TileMapEntity entity) {
            if (entity.MinLongitude.HasValue && entity.MaxLongitude.HasValue
                && entity.MinLatitude.HasValue && entity.MaxLatitude.HasValue
            ) {
                return new AgsExtent {
                    Xmin = entity.MinLongitude.Value,
                    Ymin = entity.MinLatitude.Value,
                    Xmax = entity.MaxLongitude.Value,
                    Ymax = entity.MaxLatitude.Value,
                    SpatialReference = AgsSpatialReference.WGS84
                };
            }
            return null;
        }

        public static AgsExtent GetExtent(this VectorTileEntity entity) {
            if (entity.MinLongitude.HasValue && entity.MaxLongitude.HasValue
                && entity.MinLatitude.HasValue && entity.MaxLatitude.HasValue
            ) {
                return new AgsExtent {
                    Xmin = entity.MinLongitude.Value,
                    Ymin = entity.MinLatitude.Value,
                    Xmax = entity.MaxLongitude.Value,
                    Ymax = entity.MaxLatitude.Value,
                    SpatialReference = AgsSpatialReference.WGS84
                };
            }
            return null;
        }

        public static TileMapEntity ToTileMapEntity(this TileMapCacheItem cacheItem) {
            var entity = new TileMapEntity {
                Name = cacheItem.Name,
                CacheDirectory = cacheItem.CacheDirectory,
                MapTileInfoPath = cacheItem.MapTileInfoPath,
                ContentType = cacheItem.ContentType,
                IsBundled = cacheItem.IsBundled,
                MinLevel = cacheItem.MinLevel,
                MaxLevel = cacheItem.MaxLevel
            };
            var extent = cacheItem.Extent;
            if (extent != null) {
                entity.MinLongitude = extent.Xmin;
                entity.MinLatitude = extent.Ymin;
                entity.MaxLongitude = extent.Xmax;
                entity.MaxLatitude = extent.Ymax;
            }
            return entity;
        }

        public static TileMapCacheItem ToCache(this TileMapEntity entity) {
            var cacheItem = new TileMapCacheItem {
                Name = entity.Name,
                CacheDirectory = entity.CacheDirectory,
                MapTileInfoPath = entity.MapTileInfoPath,
                ContentType = entity.ContentType,
                IsBundled = entity.IsBundled,
                ModifiedTime = null,
                MinLevel = entity.MinLevel,
                MaxLevel = entity.MaxLevel,
                Extent = entity.GetExtent()
            };
            return cacheItem;
        }

        public static VectorTileEntity ToVectorTileEntity(this TileMapCacheItem cacheItem) {
            var entity = new VectorTileEntity {
                Name = cacheItem.Name,
                Directory = cacheItem.CacheDirectory,
                MinZoom = cacheItem.MinLevel,
                MaxZoom = cacheItem.MaxLevel,
            };
            var extent = cacheItem.Extent;
            if (extent != null) {
                entity.MinLongitude = extent.Xmin;
                entity.MinLatitude = extent.Ymin;
                entity.MaxLongitude = extent.Xmax;
                entity.MaxLatitude = extent.Ymax;
            }
            return entity;
        }

        public static TileMapCacheItem ToCache(this VectorTileEntity entity) {
            var cacheItem = new TileMapCacheItem {
                Name = entity.Name,
                CacheDirectory = entity.Directory,
                MinLevel = (short)entity.MinZoom,
                MaxLevel = (short)entity.MaxZoom,
                Extent = entity.GetExtent(),
                ModifiedTime = null
            };
            return cacheItem;
        }

    }

}
