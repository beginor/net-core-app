using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.Esri {

    [JsonConverter(typeof(AgsGeometryConverter))]
    public abstract class AgsGeometry {
        public SpatialReference SpatialReference { get; set; }
        public bool HasM { get; set; }
        public bool HasZ { get; set; }
    }
}
