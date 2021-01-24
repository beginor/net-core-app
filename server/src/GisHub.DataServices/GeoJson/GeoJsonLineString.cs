namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonLineString : GeoJsonGeometry {

        public override string Type => "LineString";

        public new double[][] Coordinates {
            get { return (double[][]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
