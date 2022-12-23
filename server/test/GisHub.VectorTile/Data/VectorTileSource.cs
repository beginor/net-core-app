using System.Collections.Generic;

#nullable disable

namespace GisHub.VectorTile.Data;

public class VectorTileSource {
    public string ConnectionString { get; set; }
    public int CacheDuration { get; set; }
    public IList<VectorTileLayer> Layers { get; set; } = new List<VectorTileLayer>();
}

public class VectorTileLayer {
    public string Name { get; set; }
    public string Schema { get; set; }
    public string TableName { get; set; }
    public string IdColumn { get; set; }
    public string AttributeColumns { get; set; }
    public string GeometryColumn { get; set; }
    public int Srid { get; set; }
    public int Minzoom { get; set; } = 1;
    public int Maxzoom { get; set; } = 19;
}
