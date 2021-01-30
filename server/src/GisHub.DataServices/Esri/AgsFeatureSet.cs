using System.Collections.Generic;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsFeatureSet {
        public string DisplayFieldName { get; set; }
        public string ObjectIdFieldName { get; set; }
        public string GeometryType { get; set; }
        public IList<AgsFeature> Features { get; set; }
        public SpatialReference SpatialReference { get; set; }
        public Dictionary<string, string> FieldAliases { get; set; }
        public IList<AgsField> Fields { get; set; }
        public bool ExceededTransferLimit { get; set; }
    }

}
