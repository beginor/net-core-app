using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices {

    public static class JsonHelper {

        public static JsonSerializerOptions CreateJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict
            };
            return options;
        }

        public static JsonSerializerOptions CreateGeoJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict,
                IgnoreNullValues = true
            };
            return options;
        }

        public static JsonSerializerOptions CreateAgsJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                DictionaryKeyPolicy = null,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.Strict,
                IgnoreNullValues = true
            };
            return options;
        }
    }
}
