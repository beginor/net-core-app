using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices {

    public class CoordinateConverter : JsonConverter<double> {

        public int Digits { get; set; } = 6;

        public override double Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) {
            return reader.GetDouble();
        }

        public override void Write(
            Utf8JsonWriter writer,
            double value,
            JsonSerializerOptions options
        ) {
            if (!double.IsNaN(value)) {
                value = Math.Round(value, Digits);
            }
            writer.WriteNumberValue(value);
        }

    }

}
