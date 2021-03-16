using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.GisHub.DataServices.Esri {

    public partial class AgsQueryParam : AgsCommonParams {
        [FromQuery(Name = "where")]
        public string Where { get; set; }
        [FromQuery(Name = "geometry")]
        public string Geometry { get; set; }
        [FromQuery(Name = "inSR")]
        public int InSR { get; set; }
        [FromQuery(Name = "geometryType")]
        public string GeometryType { get; set; }
         [FromQuery(Name = "outFields")]
        public string OutFields { get; set; }
        [FromQuery(Name = "objectIds")]
        public string ObjectIds { get; set; }
        [FromQuery(Name = "outSR")]
        public int OutSR { get; set; }
        [FromQuery(Name = "spatialRel")]
        public string SpatialRel { get; set; } = AgsSpatialRelationshipType.Intersects;
        [FromQuery(Name = "returnGeometry")]
        public bool ReturnGeometry { get; set; } = true;
        [FromQuery(Name = "time")]
        public string Time { get; set; }
        [FromQuery(Name = "maxAllowableOffset")]
        public float? MaxAllowableOffset { get; set; }
        [FromQuery(Name = "returnDistinctValues")]
        public bool ReturnDistinctValues { get; set; }
        [FromQuery(Name = "orderByFields")]
        public string OrderByFields { get; set; }
        [FromQuery(Name = "resultOffset")]
        public int? ResultOffset { get; set; } = 0;
        [FromQuery(Name = "resultRecordCount")]
        public int? ResultRecordCount { get; set; } = 1000;
        [FromQuery(Name = "groupByFieldsForStatistics")]
        public string GroupByFieldsForStatistics { get; set; }
        [FromQuery(Name = "outStatistics")]
        public string OutStatistics { get; set; }
        [FromQuery(Name = "returnIdsOnly")]
        public bool ReturnIdsOnly { get; set; }
        [FromQuery(Name = "returnCountOnly")]
        public bool ReturnCountOnly { get; set; }
        [FromQuery(Name = "returnExtentOnly")]
        public bool ReturnExtentOnly { get; set; }
        [FromQuery(Name = "returnExceededLimitFeatures")]
        public bool ReturnExceededLimitFeatures { get; set; }
    }
}
