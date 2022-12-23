using System.Collections.Generic;

#nullable disable

namespace Beginor.GisHub.Geo.Esri;

public class AgsFeature {
    public IDictionary<string, object> Attributes { get; set; }
    public AgsGeometry Geometry { get; set; }
}
