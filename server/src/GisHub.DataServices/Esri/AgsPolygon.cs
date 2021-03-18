using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsPolygon : AgsGeometry {
        public double[][][] Rings { get; set; }

        public override Geometry ToGeometry() {
            throw new System.NotImplementedException();
        }

    }

}
