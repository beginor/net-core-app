#nullable disable

namespace Beginor.GisHub.Geo.GeoJson;

public class GeoJsonPoint : GeoJsonGeometry {

    public override string Type => GeoJsonGeometryType.Point;

    public double[] Coordinates { get; set; }

}
