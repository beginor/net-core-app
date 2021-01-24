namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonMultiLineString : GeoJsonGeometry {

        public override string Type => "MultiLineString";

        public new double[][][] Coordinates {
            get { return (double[][][]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
