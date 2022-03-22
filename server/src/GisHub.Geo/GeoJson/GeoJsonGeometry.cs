using System.Text.Json.Serialization;

namespace Beginor.GisHub.Geo.GeoJson; 

[JsonConverter(typeof(GeoJsonGeometryConverter))]
public abstract class GeoJsonGeometry {
    public abstract string Type { get; }
}