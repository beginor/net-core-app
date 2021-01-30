using System.Collections.Generic;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsFeature {
        public IDictionary<string, object> Attributes { get; set; }
        public AgsGeometry Geometry { get; set; }
    }

}
