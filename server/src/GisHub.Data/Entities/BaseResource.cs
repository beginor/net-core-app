using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;

namespace Beginor.GisHub.Data.Entities;

/// <summary>数据资源的基类</summary>
[Class(Schema = "public", Table = "base_resources", Where = "is_deleted = false")]
public partial class BaseResource : BaseEntity<long> {

    /// <summary>资源ID</summary>
    [Id(Name = nameof(Id), Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>资源名称</summary>
    [Property(Name = nameof(Name), Column = "name", Type = "string", NotNull = false, Length = 32)]
    public virtual string Name { get; set; }

    /// <summary>资源类别</summary>
    [ManyToOne(Name = nameof(Category), Column = "category_id", ClassType = typeof(Category), NotFound = NotFoundMode.Ignore)]
    public virtual Category Category { get; set; }

    /// <summary>资源类型</summary>
    [Property(Name = nameof(Type), Column = "type", Type = "string", NotNull = true, Length = 64)]
    public virtual string Type { get; set; }

    /// <summary>资源标签</summary>
    [Property(Name = nameof(Tags), Column = "tags", TypeType = typeof(NHibernate.Extensions.NpgSql.StringArrayType), NotNull = false)]
    public virtual string[] Tags { get; set; }

    /// <summary>创建者</summary>
    [ManyToOne(Name = nameof(Creator), Column = "creator_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser Creator { get; set; }

    /// <summary>创建时间</summary>
    [Property(Name = nameof(CreatedAt), Column = "created_at", Type = "datetime", NotNull = true)]
    public virtual DateTime CreatedAt { get; set; }

    /// <summary>更新者</summary>
    [ManyToOne(Name = nameof(Updater), Column = "updater_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser Updater { get; set; }

    /// <summary>更新时间</summary>
    [Property(Name = nameof(UpdatedAt), Column = "updated_at", Type = "datetime", NotNull = true)]
    public virtual DateTime UpdatedAt { get; set; }

    /// <summary>是否删除</summary>
    [Property(Name = nameof(IsDeleted), Column = "is_deleted", Type = "bool", NotNull = true)]
    public virtual bool IsDeleted { get; set; }
}
