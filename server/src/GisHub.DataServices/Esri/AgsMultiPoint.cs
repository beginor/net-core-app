using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsMultiPoint : AgsGeometry {
        public double[][] Points { get; set; }

        public override Geometry ToNtsGeometry() {
            var points = new Point[Points.Length];
            for (var i = 0; i < points.Length; i++) {
                var coords = Points[i];
                var point = new Point(coords[0], coords[1]);
                if (HasZ.GetValueOrDefault(false)) {
                    point.Z = coords[2];
                    if (HasM.GetValueOrDefault(false)) {
                        point.M = coords[3];
                    }
                }
                else if (HasM.GetValueOrDefault(false)) {
                    point.M = coords[2];
                }
            }
            var target = new MultiPoint(points);
            if (SpatialReference != null) {
                target.SRID = SpatialReference.Wkid;
            }
            return target;
        }

    }

}
