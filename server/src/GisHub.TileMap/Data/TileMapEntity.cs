using NHibernate.Mapping.Attributes;
using Beginor.GisHub.Data.Entities;

namespace Beginor.GisHub.TileMap.Data; 

/// <summary>切片地图</summary>
[JoinedSubclass(0, Schema = "public", Table = "tilemaps", ExtendsType = typeof(BaseResource))]
[Key(1, Column = "id")]
public partial class TileMapEntity : BaseResource {

    public TileMapEntity() { base.Type = "tilemap"; }

    /// <summary>缓存目录</summary>
    [Property(Name = "CacheDirectory", Column = "cache_directory", Type = "string", NotNull = true, Length = 512)]
    public virtual string CacheDirectory { get; set; }

    /// <summary>切片信息路径</summary>
    [Property(Name = "MapTileInfoPath", Column = "map_tile_info_path", Type = "string", NotNull = true, Length = 512)]
    public virtual string MapTileInfoPath { get; set; }

    /// <summary>内容类型</summary>
    [Property(Name = "ContentType", Column = "content_type", Type = "string", NotNull = true, Length = 64)]
    public virtual string ContentType { get; set; }

    /// <summary>目录结构</summary>
    [Property(Name = "FolderStructure", Column = "folder_structure", Type = "string", NotNull = true, Length = 16)]
    public virtual string FolderStructure { get; set; }

    /// <summary>是否为紧凑格式</summary>
    [Property(Name = "IsBundled", Column = "is_bundled", Type = "bool", NotNull = true)]
    public virtual bool IsBundled { get; set; }

    /// <summary>最小缩放级别</summary>
    [Property(Name = "MinLevel", Column = "min_level", Type = "short", NotNull = true)]
    public virtual short MinLevel { get; set; }

    /// <summary>最大缩放级别</summary>
    [Property(Name = "MaxLevel", Column = "max_level", Type = "short", NotNull = true)]
    public virtual short MaxLevel { get; set; }

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
