using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsMultiPoint : AgsGeometry {
        public double[][] Points { get; set; }

        public override Geometry ToGeometry() {
            throw new System.NotImplementedException();
        }

    }

}
