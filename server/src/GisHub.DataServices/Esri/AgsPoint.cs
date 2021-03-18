using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsPoint : AgsGeometry {
        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
        public double? M { get; set; }

        public override Geometry ToGeometry() {
            var point = new Point(X, Y);
            if (Z.HasValue) {
                point.Z = Z.Value;
            }
            if (M.HasValue) {
                point.M = M.Value;
            }
            return point;
        }

    }

}
