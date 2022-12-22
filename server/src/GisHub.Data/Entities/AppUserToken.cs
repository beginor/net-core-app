using System;
using Beginor.AppFx.Core;
using NHibernate.Extensions.NpgSql;
using NHibernate.Mapping.Attributes;

#nullable disable

namespace Beginor.GisHub.Data.Entities;

/// <summary>用户凭证</summary>
[Class(Schema = "public", Table = "app_user_tokens")]
public partial class AppUserToken : BaseEntity<long> {

    /// <summary>凭证id</summary>
    [Id(Name = "Id", Column = "id", Type = "long", Generator = "trigger-identity")]
    public override long Id { get { return base.Id; } set { base.Id = value; } }

    /// <summary>用户id</summary>
    [ManyToOne(Name = "User", Column = "user_id", ClassType = typeof(AppUser), NotFound = NotFoundMode.Ignore)]
    public virtual AppUser User { get; set; }

    /// <summary>凭证名称</summary>
    [Property(Name = "Name", Column = "name", Type = "string", NotNull = true, Length = 16)]
    public virtual string Name { get; set; }

    /// <summary>凭证值</summary>
    [Property(Name = "Value", Column = "value", Type = "string", NotNull = true, Length = 32)]
    public virtual string Value { get; set; }

    /// <summary>凭证角色</summary>
    [Property(Name = "Roles", Column = "roles", TypeType = typeof(StringArrayType), NotNull = false)]
    public virtual string[] Roles { get; set; }

    /// <summary>凭证权限</summary>
    [Property(Name = "Privileges", Column = "privileges", TypeType = typeof(StringArrayType), NotNull = false)]
    public virtual string[] Privileges { get; set; }

    /// <summary>允许的 url 地址</summary>
    [Property(Name = "Urls", Column = "urls", TypeType = typeof(StringArrayType), NotNull = false)]
    public virtual string[] Urls { get; set; }

    /// <summary>过期时间</summary>
    [Property(Name = "ExpiresAt", Column = "expires_at", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = false)]
    public virtual DateTime? ExpiresAt { get; set; }

    /// <summary>更新时间</summary>
    [Property(Name = "UpdateTime", Column = "update_time", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = true)]
    public virtual DateTime UpdateTime { get; set; }
}
