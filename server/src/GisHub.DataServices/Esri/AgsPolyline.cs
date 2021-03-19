using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsPolyline : AgsGeometry {
        public double[][][] Paths { get; set; }

        public override Geometry ToNtsGeometry() {
            var lineStrings = new LineString[Paths.Length];
            for (var i = 0; i < Paths.Length; i++) {
                var path = Paths[i];
                var points = new Coordinate[path.Length];
                for (var j = 0; j < path.Length; j++) {
                    var coords = path[j];
                    var coord = new Coordinate(coords[0], coords[1]);
                    if (HasZ.GetValueOrDefault(false)) {
                        coord.Z = coords[2];
                        if (HasM.GetValueOrDefault(false)) {
                            coord.M = coords[3];
                        }
                    }
                    else if (HasM.GetValueOrDefault(false)) {
                        coord.M = coords[2];
                    }
                    points[j] = coord;
                }
                lineStrings[i] = new LineString(points);
            }
            var target = new MultiLineString(lineStrings);
            if (SpatialReference != null) {
                target.SRID = SpatialReference.Wkid;
            }
            return target;
        }

    }

}
