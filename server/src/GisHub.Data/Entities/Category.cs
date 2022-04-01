using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.Data.Entities;

/// <summary>数据类别</summary>
[Class(Schema = "public", Table = "categories")]
public partial class Category : BaseEntity<long> {

    /// <summary>类别ID</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>类别名称</summary>
    [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 32)]
    public virtual string Name { get; set; }

    /// <summary>父类ID</summary>
    [Property(Name = "ParentId", Column = "parent_id", Type = "long", NotNull = false)]
    public virtual long? ParentId { get; set; }

    /// <summary>顺序号</summary>
    [Property(Name = "Sequence", Column = "sequence", Type = "float", NotNull = true)]
    public virtual float Sequence { get; set; }
}
