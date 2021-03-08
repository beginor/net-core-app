using System;
using System.Linq;
using System.Text.Json;
using Beginor.AppFx.Core;

namespace Beginor.GisHub.DataServices.Esri {

    partial class AgsQueryParams {
        public AgsGeometry GeometryValue {
            get {
                if (Geometry.IsNullOrEmpty()) {
                    return null;
                }
                if (GeometryType.IsNullOrEmpty()) {
                    return null;
                }
                if (GeometryType == AgsGeometryTypes.Envelope) {
                    return JsonSerializer.Deserialize<AgsExtent>(Geometry);
                }
                if (GeometryType == AgsGeometryTypes.Point) {
                    return JsonSerializer.Deserialize<AgsPoint>(Geometry);
                }
                if (GeometryType == AgsGeometryTypes.MultiPoint) {
                    return JsonSerializer.Deserialize<AgsMultiPoint>(Geometry);
                }
                if (GeometryType == AgsGeometryTypes.Polyline) {
                    return JsonSerializer.Deserialize<AgsPolyline>(Geometry);
                }
                if (GeometryType == AgsGeometryTypes.Polygon) {
                    return JsonSerializer.Deserialize<AgsPolygon>(Geometry);
                }
                return null;
            }
        }
        public string[] OutFieldsValue {
            get {
                return OutFields.Split(',');
            }
        }
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
        public string[] GroupByValue {
            get {
                if (GroupByFieldsForStatistics.IsNullOrEmpty()) {
                    return null;
                }
                return GroupByFieldsForStatistics.Split(',');
            }
        }
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
