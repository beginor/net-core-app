using System.Xml;
using NHibernate.Mapping.Attributes;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DynamicSql.Data; 

/// <summary>数据API</summary>
[JoinedSubclass(0, Schema = "public", Table = "data_apis", ExtendsType = typeof(BaseResource), Lazy = true)]
[Key(1, Column = "id")]
public partial class DataApi : BaseResource {

    public DataApi() {
        base.Type = "data_api";
    }

    /// <summary>数据源ID</summary>
    [ManyToOne(Name = "DataSource", Column = "data_source_id", ClassType = typeof(DataSource), NotFound = NotFoundMode.Ignore, Lazy = Laziness.Proxy, Fetch = FetchMode.Select)]
    public virtual DataSource DataSource { get; set; }
    /// <summary>是否向数据源写入数据</summary>
    [Property(Name = "WriteData", Column = "write_data", Type = "bool", NotNull = true)]
    public virtual bool WriteData { get; set; }
    /// <summary>数据API调用的 XML + SQL 命令</summary>
    [Property(Name = "Statement", Column = "statement", Type = "xml", NotNull = true)]
    public virtual XmlDocument Statement { get; set; }
    /// <summary>参数定义</summary>
    [Property(Name = "Parameters", Column = "parameters", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataApiParameter[]>), NotNull = false)]
    public virtual DataApiParameter[] Parameters { get; set; }
    /// <summary>API 输出列的元数据</summary>
    [Property(Name = "Columns", Column = "columns", TypeType = typeof(NHibernate.Extensions.NpgSql.JsonbType<DataServiceField[]>), NotNull = false)]
    public virtual DataServiceField[] Columns { get; set; }
    /// <summary>输出字段中的标识列</summary>
    [Property(Name = "IdColumn", Column = "id_column", Type = "string", NotNull = false, Length = 256)]
    public virtual string IdColumn { get; set; }
    /// <summary>输出字段中的空间列</summary>
    [Property(Name = "GeometryColumn", Column = "geometry_column", Type = "string", NotNull = false, Length = 256)]
    public virtual string GeometryColumn { get; set; }
}

public class DataApiParameter {
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Source { get; set; }
    public bool Required { get; set; }
}