using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Esri {

    partial class AgsQueryParam {
        [JsonIgnore]
        public AgsGeometry GeometryValue {
            get {
                if (Geometry.IsNullOrEmpty()) {
                    return null;
                }
                if (GeometryType.IsNullOrEmpty()) {
                    return null;
                }
                if (GeometryType == AgsGeometryType.Envelope) {
                    return JsonSerializer.Deserialize<AgsExtent>(Geometry);
                }
                if (GeometryType == AgsGeometryType.Point) {
                    return JsonSerializer.Deserialize<AgsPoint>(Geometry);
                }
                if (GeometryType == AgsGeometryType.MultiPoint) {
                    return JsonSerializer.Deserialize<AgsMultiPoint>(Geometry);
                }
                if (GeometryType == AgsGeometryType.Polyline) {
                    return JsonSerializer.Deserialize<AgsPolyline>(Geometry);
                }
                if (GeometryType == AgsGeometryType.Polygon) {
                    return JsonSerializer.Deserialize<AgsPolygon>(Geometry);
                }
                return null;
            }
        }
        [JsonIgnore]
        public string[] OutFieldsValue {
            get {
                return OutFields.Split(',');
            }
        }
        [JsonIgnore]
        public long[] ObjectIdsValue {
            get {
                if (ObjectIds.IsNullOrEmpty()) {
                    return null;
                }
                return ObjectIds.Split(',')
                    .Select(id => long.Parse(id))
                    .ToArray();
            }
        }
        [JsonIgnore]
        public AgsSpatialReference OutSRValue {
            get {
                if (OutSR == AgsSpatialReference.WGS84.Wkid.Value) {
                    return AgsSpatialReference.WGS84;
                }
                if (OutSR == AgsSpatialReference.WebMercator.Wkid.Value || OutSR == AgsSpatialReference.WebMercator.LatestWkid) {
                    return AgsSpatialReference.WebMercator;
                }
                return new AgsSpatialReference {
                    Wkid = OutSR,
                    LatestWkid = OutSR
                };
            }
        }
        [JsonIgnore]
        public DateTime? TimeFrom {
            get {
                if (string.IsNullOrEmpty(Time)) {
                    return null;
                }
                var arr = Time.Split(',');
                if (arr.Length < 1) {
                    return null;
                }
                if (long.TryParse(arr[0], out long start)) {
                    return start.FromUnixTime();
                }
                return null;
            }
        }
        [JsonIgnore]
        public DateTime? TimeTo {
            get {
                if (string.IsNullOrEmpty(Time)) {
                    return null;
                }
                var arr = Time.Split(',');
                if (arr.Length < 2) {
                    return null;
                }
                if (long.TryParse(arr[1], out long start)) {
                    return start.FromUnixTime();
                }
                return null;
            }
        }
        [JsonIgnore]
        public string[] GroupByValue {
            get {
                if (GroupByFieldsForStatistics.IsNullOrEmpty()) {
                    return null;
                }
                return GroupByFieldsForStatistics.Split(',');
            }
        }
        [JsonIgnore]
        public AgsOutputStatistic[] OutStatisticsValue {
            get {
                if (OutStatistics.IsNullOrEmpty()) {
                    return null;
                }
                return JsonSerializer.Deserialize<AgsOutputStatistic[]>(OutStatistics);
            }
        }
    }
}
