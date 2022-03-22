namespace Beginor.GisHub.Geo.GeoJson; 

public class GeoJsonLineString : GeoJsonGeometry {

    public override string Type => GeoJsonGeometryType.LineString;

    public double[][] Coordinates { get; set; }

}