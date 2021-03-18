using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsPolyline : AgsGeometry {
        public double[][][] Paths { get; set; }

        public override Geometry ToGeometry() {
            throw new System.NotImplementedException();
        }

    }

}
