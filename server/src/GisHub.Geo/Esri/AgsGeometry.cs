using NetTopologySuite.Geometries;

namespace Beginor.GisHub.Geo.Esri {

    // [JsonConverter(typeof(AgsGeometryConverter))]
    public abstract class AgsGeometry {
        public AgsSpatialReference SpatialReference { get; set; }
        public bool? HasM { get; set; }
        public bool? HasZ { get; set; }
        public abstract Geometry ToNtsGeometry();
    }
}
