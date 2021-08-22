using System.Text.Json.Serialization;

namespace Beginor.GisHub.Geo.Esri {
    public class AgsOutputStatistic {

        [JsonPropertyName("statisticType")]
        public string Type { get; set; }

        [JsonPropertyName("onStatisticField")]
        public string OnField { get; set; }

        [JsonPropertyName("outStatisticFieldName")]
        public string OutFieldName { get; set; }

    }
}
