namespace Beginor.GisHub.Geo.Esri {

    public class AgsSpatialReference {

        public static readonly AgsSpatialReference WGS84 = new() { Wkid = 4326, LatestWkid = 4326 };

        public static readonly AgsSpatialReference WebMercator = new() { Wkid = 102100, LatestWkid = 3857 };

        public static readonly AgsSpatialReference CGC2000 = new() { Wkid = 4490, LatestWkid = 4490 };

        public int Wkid { get; set; }

        public int? LatestWkid { get; set; }

    }

}
