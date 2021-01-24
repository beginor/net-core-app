namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonMultiPolygon : GeoJsonGeometry {

        public override string Type => "MultiPolygon";

        public new double[][][][] Coordinates {
            get { return (double[][][][]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
