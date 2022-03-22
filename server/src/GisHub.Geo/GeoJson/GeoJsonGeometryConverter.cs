using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beginor.GisHub.Geo.GeoJson; 

public class GeoJsonGeometryConverter : JsonConverter<GeoJsonGeometry> {

    private readonly CoordinateConverter coordinateConverter;

    public GeoJsonGeometryConverter(CoordinateConverter coordinateConverter) {
        this.coordinateConverter = coordinateConverter;
    }

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
        var serializerOptions = new JsonSerializerOptions(options);
        serializerOptions.Converters.Add(coordinateConverter);
        switch (value.Type) {
            case GeoJsonGeometryType.Point:
                JsonSerializer.Serialize(writer, (GeoJsonPoint)value, serializerOptions);
                break;
            case GeoJsonGeometryType.MultiPoint:
                JsonSerializer.Serialize(writer, (GeoJsonMultiPoint)value, serializerOptions);
                break;
            case GeoJsonGeometryType.LineString:
                JsonSerializer.Serialize(writer, (GeoJsonLineString)value, serializerOptions);
                break;
            case GeoJsonGeometryType.MultiLineString:
                JsonSerializer.Serialize(writer, (GeoJsonMultiLineString)value, serializerOptions);
                break;
            case GeoJsonGeometryType.Polygon:
                JsonSerializer.Serialize(writer, (GeoJsonPolygon)value, serializerOptions);
                break;
            case GeoJsonGeometryType.MultiPolygon:
                JsonSerializer.Serialize(writer, (GeoJsonMultiPolygon)value, serializerOptions);
                break;
            default:
                writer.WriteStringValue($"Unknown GeoJson Geometry {value.Type} !");
                break;
        }
    }
}