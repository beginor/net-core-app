using NetTopologySuite.Geometries;

namespace Beginor.GisHub.DataServices.Esri {

    public class AgsExtent : AgsGeometry {
        public double Xmin { get; set; }
        public double Xmax { get; set; }
        public double Ymin { get; set; }
        public double Ymax { get; set; }

        public override Geometry ToGeometry() {
            throw new System.NotImplementedException();
        }

    }

}
