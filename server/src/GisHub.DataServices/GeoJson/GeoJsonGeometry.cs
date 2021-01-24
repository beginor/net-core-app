namespace Beginor.GisHub.DataServices.GeoJson {

    public abstract class GeoJsonGeometry {
        public abstract string Type { get; }
        
        public object Coordinates { get; set; }
    }

}
