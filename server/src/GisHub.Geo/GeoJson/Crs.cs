using System.Text.Json.Serialization;

namespace Beginor.GisHub.Geo.GeoJson {

    public class Crs {
        public string Type { get; set; }
        public CrsProperties Properties { get; set; }
    }
    public class CrsProperties {
        public string Name { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
        /// <summary>Wkid</summary>
        public int Code { get; set; }
    }
}
