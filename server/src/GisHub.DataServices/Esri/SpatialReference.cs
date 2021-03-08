namespace Beginor.GisHub.DataServices.Esri {

    public class AgsSpatialReference {

        public static readonly AgsSpatialReference WGS84 = new AgsSpatialReference {
            Wkid = 4326,
            LatestWkid = 4326
        };

        public static readonly AgsSpatialReference WebMercator = new AgsSpatialReference {
            Wkid = 102100,
            LatestWkid = 3857
        };

        public int? Wkid { get; set; }

        public int? LatestWkid { get; set; }

    }

}
