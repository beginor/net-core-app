using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Esri {

    partial class AgsQueryParam {

        private AgsGeometry geometry = null;

        [JsonIgnore]
        public AgsGeometry GeometryValue {
            get {
                if (Geometry.IsNullOrEmpty()) {
                    return null;
                }
                if (GeometryType.IsNullOrEmpty()) {
                    return null;
                }
                if (geometry != null) {
                    return geometry;
                }
                var options = JsonFactory.CreateAgsJsonSerializerOptions();
                if (GeometryType == AgsGeometryType.Envelope) {
                    geometry = JsonSerializer.Deserialize<AgsExtent>(Geometry, options);
                }
                if (GeometryType == AgsGeometryType.Point) {
                    geometry = JsonSerializer.Deserialize<AgsPoint>(Geometry, options);
                }
                if (GeometryType == AgsGeometryType.MultiPoint) {
                    geometry = JsonSerializer.Deserialize<AgsMultiPoint>(Geometry, options);
                }
                if (GeometryType == AgsGeometryType.Polyline) {
                    geometry = JsonSerializer.Deserialize<AgsPolyline>(Geometry, options);
                }
                if (GeometryType == AgsGeometryType.Polygon) {
                    geometry = JsonSerializer.Deserialize<AgsPolygon>(Geometry, options);
                }
                if (geometry != null && geometry.SpatialReference == null && InSR > 0) {
                    geometry.SpatialReference = new AgsSpatialReference { Wkid = InSR };
                }
                return geometry;
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
                if (OutSR == AgsSpatialReference.WGS84.Wkid) {
                    return AgsSpatialReference.WGS84;
                }
                if (OutSR == AgsSpatialReference.WebMercator.Wkid || OutSR == AgsSpatialReference.WebMercator.LatestWkid) {
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
