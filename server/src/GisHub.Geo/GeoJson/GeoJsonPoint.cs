using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.Geo.GeoJson; 

public class GeoJsonPoint : GeoJsonGeometry {

    public override string Type => GeoJsonGeometryType.Point;

    public double[] Coordinates { get; set; }

}