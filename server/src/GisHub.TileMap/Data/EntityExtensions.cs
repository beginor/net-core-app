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

    }

}
