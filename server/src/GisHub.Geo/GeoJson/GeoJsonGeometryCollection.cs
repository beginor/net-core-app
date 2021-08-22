namespace Beginor.GisHub.Geo.GeoJson {

    public class GeoJsonGeometryCollection : GeoJsonGeometry {
        public override string Type => "GeometryCollection";
        public GeoJsonGeometry[] Geometries { get; set; }

    }

}
