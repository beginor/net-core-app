using NHibernate.Mapping.Attributes;
using Beginor.GisHub.Data.Entities;

namespace Beginor.GisHub.TileMap.Data; 

/// <summary>矢量切片包</summary>
[JoinedSubclass(0, Schema = "public", Table = "vectortiles", ExtendsType = typeof(BaseResource))]
[Key(1, Column = "id")]
public partial class VectorTileEntity : BaseResource {

    public VectorTileEntity() { base.Type = "vectortile"; }

    /// <summary>矢量切片包目录</summary>
    [Property(Name = "Directory", Column = "directory", Type = "string", NotNull = true, Length = 512)]
    public virtual string Directory { get; set; }

    /// <summary>最小缩放级别</summary>
    [Property(Name = "MinZoom", Column = "min_zoom", Type = "short", NotNull = false)]
    public virtual short? MinZoom { get; set; }

    /// <summary>最大缩放级别</summary>
    [Property(Name = "MaxZoom", Column = "max_zoom", Type = "short", NotNull = false)]
    public virtual short? MaxZoom { get; set; }

    [Property(Name = "DefaultStyle", Column = "default_style", Type = "string", Length = 32, NotNull = false)]
    public virtual string DefaultStyle { get; set; }
    /// <summary>最小纬度</summary>
    [Property(Name = "MinLatitude", Column = "min_latitude", Type = "double", NotNull = false)]
    public virtual double? MinLatitude { get; set; }

    /// <summary>最大纬度</summary>
    [Property(Name = "MaxLatitude", Column = "max_latitude", Type = "double", NotNull = false)]
    public virtual double? MaxLatitude { get; set; }

    /// <summary>最小经度</summary>
    [Property(Name = "MinLongitude", Column = "min_longitude", Type = "double", NotNull = false)]
    public virtual double? MinLongitude { get; set; }

    /// <summary>最大经度</summary>
    [Property(Name = "MaxLongitude", Column = "max_longitude", Type = "double", NotNull = false)]
    public virtual double? MaxLongitude { get; set; }
}
