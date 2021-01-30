using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonFeatureCollection {
        public string Type => "FeatureCollection";
        public double[] Bbox { get; set; }
        public IList<GeoJsonFeature> Features { get; set; }
        public Crs Crs { get; set; }
        public bool ExceededTransferLimit { get; set; }
    }

}
