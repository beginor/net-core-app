using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices {

    public class JsonSerializerOptionsFactory {

        public JsonSerializerOptions CreateJsonSerializerOptions() {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            return options;
        }

        public JsonSerializerOptions CreateGeoJsonSerializerOptions() {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            return options;
        }

        public JsonSerializerOptions CreateAgsJsonSerializerOptions() {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
                DictionaryKeyPolicy = null,
                IgnoreNullValues = true
            };
            return options;
        }
    }
}
