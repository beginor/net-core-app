using System.Text.Json;
using System.Text.Json.Serialization;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;

namespace Beginor.GisHub.DataServices {

    public class JsonSerializerOptionsFactory {

        public JsonSerializerOptionsFactory(CoordinateConverter coordinateConverter) {
            JsonSerializerOptions = new (JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            GeoJsonSerializerOptions = new (JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            GeoJsonSerializerOptions.Converters.Add(new GeoJsonGeometryConverter(coordinateConverter));
            AgsJsonSerializerOptions = new (JsonSerializerDefaults.Web) {
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
