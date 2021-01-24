using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonFeature {
        public object Id { get; set; }
        public IDictionary<string, object> Properties { get; set; }
        public string Type => "Feature";
        public double[] Bbox { get; set; }
        public GeoJsonGeometry Geometry { get; set; }
    }

}
