using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

#nullable disable

namespace Beginor.GisHub.Data.Entities;

/// <summary>系统权限</summary>
[Class(Schema = "public", Table = "app_privileges")]
public partial class AppPrivilege : BaseEntity<long> {

    /// <summary>权限ID</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id {
        get { return base.Id; }
        set { base.Id = value; }
    }

    /// <summary>权限模块</summary>
    [Property(Name = "Module", Column = "module", Type = "string", NotNull = true, Length = 32)]
    public virtual string Module { get; set; }
    /// <summary>权限名称( Identity 的策略名称)</summary>
    [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 64)]
    public virtual string Name { get; set; }
    /// <summary>权限描述</summary>
    [Property(Name = "Description", Column = "description", Type = "string", NotNull = false, Length = 128)]
    public virtual string Description { get; set; }
    /// <summary>是否必须。 与代码中的 Authorize 标记对应的权限为必须的权限， 否则为可选的。</summary>
    [Property(Name = "IsRequired", Column = "is_required", Type = "bool", NotNull = true)]
    public virtual bool IsRequired { get; set; }

}
