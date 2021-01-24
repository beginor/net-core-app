namespace Beginor.GisHub.DataServices.GeoJson {

    public class GeoJsonGeometryCollection : GeoJsonGeometry {
        
        public override string Type {
            get { return "GeometryCollection"; }
        }
        
        public GeoJsonGeometry[] Geometries { get; set; }

    }

}
