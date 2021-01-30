namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonPolygon : GeoJsonGeometry {

        public override string Type => GeoJsonGeometryType.Polygon;

        public double[][][] Coordinates { get; set; }

    }

}
