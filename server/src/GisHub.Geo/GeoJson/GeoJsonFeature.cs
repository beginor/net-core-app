using System.Collections.Generic;

#nullable disable

namespace Beginor.GisHub.Geo.GeoJson;

public class GeoJsonFeature {
    public object Id { get; set; }
    public IDictionary<string, object> Properties { get; set; }
    public string Type => "Feature";
    public double[] Bbox { get; set; }
    public GeoJsonGeometry Geometry { get; set; }
}
