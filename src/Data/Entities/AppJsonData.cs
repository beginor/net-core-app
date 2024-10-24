using System;
using System.Text.Json;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

#nullable disable

namespace Beginor.NetCoreApp.Data.Entities;

/// <summary>json 数据</summary>
[Class(Schema = "public", Table = "app_json_data")]
public partial class AppJsonData : BaseEntity<long> {

    /// <summary>json 数据id</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>json值</summary>
    [Property(Name = "Value", Column = "value", Type = "jsonb", NotNull = true)]
    public virtual JsonElement Value { get; set; }

    /// <summary>业务ID</summary>
    [Property(Name = nameof(BusinessId), Column = "business_id", Type = "long", NotNull = true)]
    public virtual long BusinessId { get; set; } = 0L;

    /// <summary>名称</summary>
    [Property(Name = nameof(Name), Column = "name", Type = "string", Length = 64, NotNull = true)]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>更新时间</summary>
    [Property(Name = "UpdatedAt", Column = "updated_at", Type = "timestamp", NotNull = true)]
    public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;

}
