namespace Beginor.GisHub.DataServices.GeoJson {
    public class GeoJsonPoint : GeoJsonGeometry {

        public override string Type => "Point";

        public new double[] Coordinates {
            get { return (double[]) base.Coordinates; }
            set { base.Coordinates = value; }
        }

    }

}
