namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonMultiPoint : GeoJsonGeometry {

        public override string Type => "MultiPoint";

        public new double[][] Coordinates {
            get { return (double[][]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
