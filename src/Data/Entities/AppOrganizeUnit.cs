using System;
using NHibernate.Mapping.Attributes;
using Beginor.AppFx.Core;

#nullable disable
namespace Beginor.NetCoreApp.Data.Entities;

/// <summary>组织单元</summary>
[Class(Schema = "public", Table = "app_organize_unit", Where = "is_deleted = false")]
public partial class AppOrganizeUnit : BaseEntity<long> {
    /// <summary>组织单元ID</summary>
    [Id(Name = nameof(Id), Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }
    /// <summary>上级组织单元 ID</summary>
    [Property(Name = nameof(ParentId), Column = "parent_id", Type = "long", NotNull = false)]
    public virtual long? ParentId { get; set; }
    /// <summary>组织单元编码</summary>
    [Property(Name = nameof(Code), Column = "code", Type = "string", NotNull = true, Length = 32)]
    public virtual string Code { get; set; }
    /// <summary>组织单元名称</summary>
    [Property(Name = nameof(Name), Column = "name", Type = "string", NotNull = true, Length = 32)]
    public virtual string Name { get; set; }
    /// <summary>组织单元说明</summary>
    [Property(Name = nameof(Description), Column = "description", Type = "string", NotNull = false, Length = 128)]
    public virtual string Description { get; set; }
    /// <summary>组织机构排序</summary>
    [Property(Name = nameof(Sequence), Column = "sequence", Type = "float", NotNull = true)]
    public virtual float Sequence { get; set; }
    /// <summary>创建者ID</summary>
    [ManyToOne(Name = "Creator", Column = "creator_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser Creator { get; set; }
    /// <summary>创建时间</summary>
    [Property(Name = "CreatedAt", Column = "created_at", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = true)]
    public virtual DateTime CreatedAt { get; set; }
    /// <summary>更新者ID</summary>
    [ManyToOne(Name = "Updater", Column = "updater_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser Updater { get; set; }
    /// <summary>更新时间</summary>
    [Property(Name = "UpdatedAt", Column = "updated_at", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = true)]
    public virtual DateTime UpdatedAt { get; set; }
    /// <summary>是否删除</summary>
    [Property(Name = "IsDeleted", Column = "is_deleted", Type = "bool", NotNull = true)]
    public virtual bool IsDeleted { get; set; }
}
