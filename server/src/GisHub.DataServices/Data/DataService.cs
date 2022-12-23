using NHibernate.Mapping.Attributes;
using Beginor.GisHub.Data.Entities;

#nullable disable

namespace Beginor.GisHub.DataServices.Data;

/// <summary>数据服务</summary>
[JoinedSubclass(0, Schema = "public", Table = "data_services", ExtendsType = typeof(BaseResource))]
[Key(1, Column = "id")]
public partial class DataService : BaseResource {

    public DataService() {
        base.Type = "data_service";
    }

    /// <summary>数据源id</summary>
    [ManyToOne(Name = "DataSource", Column = "data_source_id", ClassType = typeof(DataSource), NotFound = NotFoundMode.Ignore)]
    public virtual DataSource DataSource { get; set; }

    /// <summary>数据表/视图架构</summary>
    [Property(Name = "Schema", Column = "schema", Type = "string", NotNull = false, Length = 16)]
    public virtual string Schema { get; set; }

    /// <summary>数据表/视图名称</summary>
    [Property(Name = "TableName", Column = "table_name", Type = "string", NotNull = true, Length = 64)]
    public virtual string TableName { get; set; }

    /// <summary>数据服务公开的字段列表</summary>
    [Property(Name = "Fields", Column = "fields", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataServiceField[]>), NotNull = false)]
    public virtual DataServiceField[] Fields { get; set; }

    /// <summary>主键列名称</summary>
    [Property(Name = "PrimaryKeyColumn", Column = "primary_key_column", Type = "string", NotNull = true, Length = 256)]
    public virtual string PrimaryKeyColumn { get; set; }

    /// <summary>显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。</summary>
    [Property(Name = "DisplayColumn", Column = "display_column", Type = "string", NotNull = true, Length = 256)]
    public virtual string DisplayColumn { get; set; }

    /// <summary>空间列</summary>
    [Property(Name = "GeometryColumn", Column = "geometry_column", Type = "string", NotNull = false, Length = 256)]
    public virtual string GeometryColumn { get; set; }

    /// <summary>预置过滤条件</summary>
    [Property(Name = "PresetCriteria", Column = "preset_criteria", Type = "string", NotNull = false, Length = 128)]
    public virtual string PresetCriteria { get; set; }

    /// <summary>默认排序</summary>
    [Property(Name = "DefaultOrder", Column = "default_order", Type = "string", NotNull = false, Length = 128)]
    public virtual string DefaultOrder { get; set; }

    /// <summary>是否支持矢量切片格式</summary>
    [Property(Name = "SupportMvt", Column = "support_mvt", Type = "bool", NotNull = false)]
    public virtual bool? SupportMvt { get; set; }

    /// <summary>矢量切片最小级别</summary>
    [Property(Name = "MvtMinZoom", Column = "mvt_min_zoom", Type = "int", NotNull = false)]
    public virtual int? MvtMinZoom { get; set; }

    /// <summary>矢量切片最大级别</summary>
    [Property(Name = "MvtMaxZoom", Column = "mvt_max_zoom", Type = "int", NotNull = false)]
    public virtual int? MvtMaxZoom { get; set; }

    /// <summary>矢量切片缓存时间(秒)</summary>
    [Property(Name = "MvtCacheDuration", Column = "mvt_cache_duration", Type = "int", NotNull = false)]
    public virtual int? MvtCacheDuration { get; set; }
}

public class DataServiceField {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int Length { get; set; }
    public bool Nullable { get; set; }
    public bool Editable { get; set; }
}
