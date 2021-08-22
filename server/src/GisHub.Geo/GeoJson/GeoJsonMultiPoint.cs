namespace Beginor.GisHub.Geo.GeoJson {

    public class GeoJsonMultiPoint : GeoJsonGeometry {

        public override string Type => GeoJsonGeometryType.MultiPoint;

        public double[][] Coordinates { get; set; }

    }

}
