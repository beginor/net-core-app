using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.DataServices.GeoJson {

    [JsonConverter(typeof(GeoJsonGeometryConverter))]
    public abstract class GeoJsonGeometry {
        public abstract string Type { get; }
    }

    public static class GeoJsonGeometryType {
        public const string Point = "Point";
        public const string MultiPoint = "MultiPoint";
        public const string LineString = "LineString";
        public const string MultiLineString = "MultiLineString";
        public const string Polygon = "Polygon";
        public const string MultiPolygon = "MultiPolygon";
    }

    public class GeoJsonGeometryConverter : JsonConverter<GeoJsonGeometry> {

        public override GeoJsonGeometry Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            GeoJsonGeometry value,
            JsonSerializerOptions options
        ) {
            switch (value.Type) {
                case GeoJsonGeometryType.Point:
                    JsonSerializer.Serialize(writer, (GeoJsonPoint)value, options);
                    break;
                case GeoJsonGeometryType.MultiPoint:
                    JsonSerializer.Serialize(writer, (GeoJsonMultiPoint)value, options);
                    break;
                case GeoJsonGeometryType.LineString:
                    JsonSerializer.Serialize(writer, (GeoJsonLineString)value, options);
                    break;
                case GeoJsonGeometryType.MultiLineString:
                    JsonSerializer.Serialize(writer, (GeoJsonMultiLineString)value, options);
                    break;
                case GeoJsonGeometryType.Polygon:
                    JsonSerializer.Serialize(writer, (GeoJsonPolygon)value, options);
                    break;
                case GeoJsonGeometryType.MultiPolygon:
                    JsonSerializer.Serialize(writer, (GeoJsonMultiPolygon)value, options);
                    break;
                default:
                    writer.WriteStringValue($"Unknown GeoJson Geometry {value.Type} !");
                    break;
            }
        }
    }

}
