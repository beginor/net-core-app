using Microsoft.AspNetCore.Mvc;

namespace Beginor.GisHub.DataServices.Models {

    public class CountParam {
        [FromQuery(Name = "$where")]
        public string Where { get; set; }
        public bool CheckGeometryColumn { get; set; } = true;
    }

    public class DistinctParam : CountParam {
        [FromQuery(Name = "$select")]
        public string Select { get; set; }
        [FromQuery(Name = "$orderBy")]
        public string OrderBy { get; set; }
    }

    public class GeoJsonParam : DistinctParam {
        [FromQuery(Name = "$skip")]
        public int Skip { get; set; } = 0;
        [FromQuery(Name = "$take")]
        public int Take { get; set; } = 10;
    }
    
    public class AgsJsonParam : DistinctParam {
        [FromQuery(Name = "$skip")]
        public int Skip { get; set; } = 0;
        [FromQuery(Name = "$take")]
        public int Take { get; set; } = 10;
    }

    public class ReadDataParam : DistinctParam {
        [FromQuery(Name = "$groupBy")]
        public string GroupBy { get; set; }
        [FromQuery(Name = "$skip")]
        public int Skip { get; set; } = 0;
        [FromQuery(Name = "$take")]
        public int Take { get; set; } = 10;

    }

    public class PivotParam : DistinctParam {
        [FromQuery(Name = "$aggregate")]
        public string Aggregate { get; set; }
        [FromQuery(Name = "$field")]
        public string Field { get; set; }
        [FromQuery(Name = "$value")]
        public string Value { get; set; }
    }

}
