using NetTopologySuite.Geometries;

#nullable disable

namespace Beginor.GisHub.Geo.Esri;

public class AgsPoint : AgsGeometry {
    public double X { get; set; }
    public double Y { get; set; }
    public double? Z { get; set; }
    public double? M { get; set; }

    public override Geometry ToNtsGeometry() {
        var target = new Point(X, Y);
        if (Z.HasValue) {
            target.Z = Z.Value;
        }
        if (M.HasValue) {
            target.M = M.Value;
        }
        if (SpatialReference != null) {
            target.SRID = SpatialReference.Wkid;
        }
        return target;
    }

}
