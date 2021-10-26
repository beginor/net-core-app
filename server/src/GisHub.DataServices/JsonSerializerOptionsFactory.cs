using System.Text.Json;
using System.Text.Json.Serialization;
using Beginor.GisHub.Common;
using Beginor.GisHub.Geo;
using Beginor.GisHub.Geo.Esri;
using Beginor.GisHub.Geo.GeoJson;

namespace Beginor.GisHub.DataServices {

    public class JsonSerializerOptionsFactory {

        public JsonSerializerOptionsFactory(CommonOption commonOption) {
            var coordinateConverter = new CoordinateConverter {
                Digits = commonOption.Output.Coordinate.Digits
            };
            JsonSerializerOptions = new(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            GeoJsonSerializerOptions = new(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            GeoJsonSerializerOptions.Converters.Add(new GeoJsonGeometryConverter(coordinateConverter));
            AgsJsonSerializerOptions = new(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            AgsJsonSerializerOptions.Converters.Add(new AgsGeometryConverter(coordinateConverter));
        }

        public JsonSerializerOptions JsonSerializerOptions { get; }

        public JsonSerializerOptions GeoJsonSerializerOptions { get; }

        public JsonSerializerOptions AgsJsonSerializerOptions { get; }

    }
}
