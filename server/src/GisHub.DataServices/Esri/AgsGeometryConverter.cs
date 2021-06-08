using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsGeometryConverter : JsonConverter<AgsGeometry> {

        private readonly CoordinateConverter coordinateConverter;

        public AgsGeometryConverter(CoordinateConverter coordinateConverter) {
            this.coordinateConverter = coordinateConverter;
        }

        public override AgsGeometry Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            AgsGeometry value,
            JsonSerializerOptions options
        ) {
            var serializerOptions = new JsonSerializerOptions(options);
            serializerOptions.Converters.Add(coordinateConverter);
            if (value is AgsPoint point) {
                JsonSerializer.Serialize(writer, point, serializerOptions);
            }
            else if (value is AgsMultiPoint points) {
                JsonSerializer.Serialize(writer, points, serializerOptions);
            }
            else if (value is AgsPolyline line) {
                JsonSerializer.Serialize(writer, line, serializerOptions);
            }
            else if (value is AgsPolygon polygon) {
                JsonSerializer.Serialize(writer, polygon, serializerOptions);
            }
            else if (value is AgsExtent extent) {
                JsonSerializer.Serialize(writer, extent, serializerOptions);
            }
            else {
                writer.WriteStringValue($"Unkonwn geometry type {value.GetType()} !");
            }
        }

    }
}
