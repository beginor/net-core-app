using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsGeometryConverter : JsonConverter<AgsGeometry> {

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
            if (value is AgsPoint point) {
                JsonSerializer.Serialize(writer, point, options);
            }
            else if (value is AgsMultiPoint points) {
                JsonSerializer.Serialize(writer, points, options);
            }
            else if (value is AgsPolyline line) {
                JsonSerializer.Serialize(writer, line, options);
            }
            else if (value is AgsPolygon polygon) {
                JsonSerializer.Serialize(writer, polygon, options);
            }
            else if (value is AgsExtent extent) {
                JsonSerializer.Serialize(writer, extent, options);
            }
            else {
                writer.WriteStringValue($"Unkonwn geometry type {value.GetType()} !");
            }
        }

    }
}
