namespace Beginor.GisHub.Geo.GeoJson {

    public class GeoJsonMultiLineString : GeoJsonGeometry {

        public override string Type => GeoJsonGeometryType.MultiLineString;

        public double[][][] Coordinates { get; set; }

    }

}
