using NetTopologySuite.Geometries;

#nullable disable

namespace Beginor.GisHub.Geo.Esri;

public class AgsExtent : AgsGeometry {
    public double Xmin { get; set; }
    public double Xmax { get; set; }
    public double Ymin { get; set; }
    public double Ymax { get; set; }

    public override Geometry ToNtsGeometry() {
        var points = new Coordinate[5];
        points[0] = new Coordinate(Xmin, Ymin);
        points[1] = new Coordinate(Xmin, Ymax);
        points[2] = new Coordinate(Xmax, Ymax);
        points[3] = new Coordinate(Xmax, Ymin);
        points[4] = new Coordinate(Xmin, Ymin);
        var target = new Polygon(new LinearRing(points));
        if (SpatialReference != null) {
            target.SRID = SpatialReference.Wkid;
        }
        return target;
    }

}
