using NHibernate.Mapping.Attributes;
using Beginor.GisHub.Data.Entities;

#nullable disable

namespace Beginor.GisHub.Slpk.Data;

/// <summary>slpk 航拍模型</summary>
[JoinedSubclass(0, Schema = "public", Table = "slpks", ExtendsType = typeof(BaseResource))]
[Key(1, Column = "id")]
public partial class SlpkEntity : BaseResource {

    public SlpkEntity() { base.Type = "slpk"; }

    /// <summary>航拍模型目录</summary>
    [Property(Name = "Directory", Column = "directory", Type = "string", NotNull = true, Length = 512)]
    public virtual string Directory { get; set; }

    /// <summary>模型经度</summary>
    [Property(Name = "Longitude", Column = "longitude", Type = "double", NotNull = true)]
    public virtual double Longitude { get; set; }

    /// <summary>模型纬度</summary>
    [Property(Name = "Latitude", Column = "latitude", Type = "double", NotNull = true)]
    public virtual double Latitude { get; set; }

    /// <summary>模型海拔高度</summary>
    [Property(Name = "Elevation", Column = "elevation", Type = "double", NotNull = true)]
    public virtual double Elevation { get; set; }
}
