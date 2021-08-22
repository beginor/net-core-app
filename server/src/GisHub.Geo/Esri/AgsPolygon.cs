using NetTopologySuite.Geometries;

namespace Beginor.GisHub.Geo.Esri {

    public class AgsPolygon : AgsGeometry {
        public double[][][] Rings { get; set; }

        public override Geometry ToNtsGeometry() {
            var linearRings = new LinearRing[Rings.Length];
            if (linearRings.Length == 1) {
                var polygon = new Polygon(linearRings[0]);
                if (SpatialReference != null) {
                    polygon.SRID = SpatialReference.Wkid;
                }
                return polygon;
            }
            var polygons = new Polygon[linearRings.Length];
            for (var i = 0; i < linearRings.Length; i++) {
                var ring = Rings[i];
                var points = new Coordinate[ring.Length];
                for (var j = 0; j < ring.Length; j++) {
                    var coords = ring[j];
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
                var linearRing = new LinearRing(points);
                polygons[i] = new Polygon(linearRing);
            }
            var multiPolygon = new MultiPolygon(polygons);
            if (SpatialReference != null) {
                multiPolygon.SRID = SpatialReference.Wkid;
            }
            return multiPolygon;
        }

    }

}
