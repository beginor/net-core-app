using System;
using System.Text;
using System.Text.Json;

namespace Beginor.GisHub.DataServices.Esri {
    public class AgsDrawingInfo {

        public static JsonElement CreateDefaultDrawingInfo(string geometryType) {
            JsonElement drawingInfo;
            switch (geometryType) {
                case AgsGeometryType.Point:
                case AgsGeometryType.MultiPoint:
                    drawingInfo = CreatePointDrawingInfo();
                    break;
                case AgsGeometryType.Polyline:
                    drawingInfo = CreateLineDrawingInfo();
                    break;
                case AgsGeometryType.Polygon:
                    drawingInfo = CreatePolygonDrawingInfo();
                    break;
                default:
                    throw new NotSupportedException(
                        $"Not supported geometry type {geometryType} !"
                    );
            }
            return drawingInfo;
        }

        private static JsonElement CreatePolygonDrawingInfo() {
            return JsonDocument.Parse(new StringBuilder()
                .AppendLine("{")
                .AppendLine("  \"renderer\": {")
                .AppendLine("   \"type\": \"simple\",")
                .AppendLine("   \"symbol\": {")
                .AppendLine("    \"type\": \"esriSFS\",")
                .AppendLine("    \"style\": \"esriSFSSolid\",")
                .AppendLine("    \"color\": [255, 0, 0, 50],")
                .AppendLine("    \"outline\": {")
                .AppendLine("     \"type\": \"esriSLS\",")
                .AppendLine("     \"style\": \"esriSLSSolid\",")
                .AppendLine("     \"color\": [255, 0, 0, 255],")
                .AppendLine("     \"width\": 1")
                .AppendLine("    }")
                .AppendLine("   },")
                .AppendLine("   \"label\": \"\",")
                .AppendLine("   \"description\": \"\"")
                .AppendLine("  },")
                .AppendLine("  \"transparency\": 0,")
                .AppendLine("  \"labelingInfo\": null")
                .AppendLine(" }")
                .ToString()
            ).RootElement;
        }

        private static JsonElement CreateLineDrawingInfo() {
            return JsonDocument.Parse(new StringBuilder()
                .AppendLine("{")
                .AppendLine("  \"renderer\": {")
                .AppendLine("   \"type\": \"simple\",")
                .AppendLine("   \"symbol\": {")
                .AppendLine("    \"type\": \"esriSLS\",")
                .AppendLine("    \"style\": \"esriSLSSolid\",")
                .AppendLine("    \"color\": [255, 0, 0, 255],")
                .AppendLine("    \"width\": 1")
                .AppendLine("   },")
                .AppendLine("   \"label\": \"\",")
                .AppendLine("   \"description\": \"\"")
                .AppendLine("  },")
                .AppendLine("  \"transparency\": 0,")
                .AppendLine("  \"labelingInfo\": null")
                .AppendLine("}")
                .ToString()
            ).RootElement;
        }

        private static JsonElement CreatePointDrawingInfo() {
            return JsonDocument.Parse(new StringBuilder()
                .AppendLine("{")
                .AppendLine("  \"renderer\": {")
                .AppendLine("   \"type\": \"simple\",")
                .AppendLine("   \"symbol\": {")
                .AppendLine("    \"type\": \"esriSMS\",")
                .AppendLine("    \"style\": \"esriSMSCircle\",")
                .AppendLine("    \"color\": [255, 0, 0, 255],")
                .AppendLine("    \"size\": 8,")
                .AppendLine("    \"angle\": 0,")
                .AppendLine("    \"xoffset\": 0,")
                .AppendLine("    \"yoffset\": 0,")
                .AppendLine("    \"outline\": {")
                .AppendLine("     \"color\": [0, 0, 0, 255],")
                .AppendLine("     \"width\": 1")
                .AppendLine("    }")
                .AppendLine("   },")
                .AppendLine("   \"label\": \"\",")
                .AppendLine("   \"description\": \"\"")
                .AppendLine("  },")
                .AppendLine("  \"transparency\": 0,")
                .AppendLine("  \"labelingInfo\": null")
                .AppendLine("}")
                .ToString()
            ).RootElement;
        }
    }

}
