namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonMultiPoint : GeoJsonGeometry {

        public override string Type => GeoJsonGeometryType.MultiPoint;

        public double[][] Coordinates { get; set; }

    }

}
