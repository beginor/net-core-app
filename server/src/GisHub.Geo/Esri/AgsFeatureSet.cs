using System.Collections.Generic;

namespace Beginor.GisHub.Geo.Esri {

    public class AgsFeatureSet {
        public string DisplayFieldName { get; set; }
        public string ObjectIdFieldName { get; set; }
        public string GeometryType { get; set; }
        public IList<AgsFeature> Features { get; set; }
        public AgsSpatialReference SpatialReference { get; set; }
        public IDictionary<string, string> FieldAliases { get; set; }
        public IList<AgsField> Fields { get; set; }
        public bool? ExceededTransferLimit { get; set; }
        public long? Count { get; set; }
        public long[] ObjectIds { get; set; }
        public AgsExtent Extent { get; set; }
    }

}
