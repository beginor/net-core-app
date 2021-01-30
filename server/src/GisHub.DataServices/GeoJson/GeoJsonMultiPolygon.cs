namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonMultiPolygon : GeoJsonGeometry {

        public override string Type => GeoJsonGeometryType.MultiPolygon;

        public double[][][][] Coordinates { get; set; }
    }

}
