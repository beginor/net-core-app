namespace Beginor.GisHub.DataServices.Esri {

    public class SpatialReference {
        
        public static readonly SpatialReference WGS84 = new SpatialReference {
            Wkid = 4326,
            LatestWkid = 4326
        };
        
        public static readonly SpatialReference WebMercator = new SpatialReference {
            Wkid = 102100,
            LatestWkid = 3857
        };
        
        public int? Wkid { get; set; }
        
        public int? LatestWkid { get; set; }

    }

}
