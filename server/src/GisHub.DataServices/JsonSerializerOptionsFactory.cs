using System.Text.Json;
using System.Text.Json.Serialization;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.GeoJson;

namespace Beginor.GisHub.DataServices {

    public class JsonSerializerOptionsFactory {

        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly JsonSerializerOptions geoJsonSerializerOptions;
        private readonly JsonSerializerOptions agsJsonSerializerOptions;

        public JsonSerializerOptionsFactory(CoordinateConverter coordinateConverter) {
            jsonSerializerOptions = new (JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            geoJsonSerializerOptions = new (JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            geoJsonSerializerOptions.Converters.Add(new GeoJsonGeometryConverter(coordinateConverter));
            agsJsonSerializerOptions = new (JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            agsJsonSerializerOptions.Converters.Add(new AgsGeometryConverter(coordinateConverter));
        }

        public JsonSerializerOptions JsonSerializerOptions => jsonSerializerOptions;

        public JsonSerializerOptions GeoJsonSerializerOptions => geoJsonSerializerOptions;

        public JsonSerializerOptions AgsJsonSerializerOptions => agsJsonSerializerOptions;

    }
}
