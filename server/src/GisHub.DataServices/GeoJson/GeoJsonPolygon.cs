namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonPolygon : GeoJsonGeometry {

        public override string Type => "Polygon";

        public double[][][] Coordinates {
            get { return (double[][][]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
