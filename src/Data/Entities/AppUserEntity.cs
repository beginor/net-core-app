using System;
using NHibernate.Mapping.Attributes;

using NHIdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

#nullable disable

namespace Beginor.NetCoreApp.Data.Entities;

[JoinedSubclass(0, Schema = "public", Table = "app_users", ExtendsType = typeof(NHIdentityUser))]
[Key(1, Column = "id")]
public class AppUserEntity : NHIdentityUser {

    [Property(Column = "created_at", Type = "timestamp", NotNull = true, Generated = PropertyGeneration.Insert, Update = false, Insert = false)]
    public virtual DateTime CreatedAt { get; set; }

    [Property(Column = "last_login", Type = "timestamp", NotNull = false)]
    public virtual DateTime? LastLogin { get; set; }

    [Property(Column = "login_count", Type = "int", NotNull = true)]
    public virtual int LoginCount { get; set; }

    [ManyToOne(Name = "OrganizeUnit", Column = "organize_unit_id", ClassType = typeof(AppOrganizeUnitEntity), NotNull = true, Lazy = Laziness.False)]
    public virtual AppOrganizeUnitEntity OrganizeUnit { get; set; }

    [Property(Name = nameof(DisplayName), Column = "display_name", Type = "string", NotNull = false, Length = 64)]
    public virtual string DisplayName { get; set; }

    /// <summary>姓</summary>
    [Property(Name = nameof(Surname), Column = "surname", Type = "string", NotNull = false, Length = 32)]
    public virtual string Surname { get; set; }

    /// <summary>名</summary>
    [Property(Name = nameof(GivenName), Column = "given_name", Type = "string", NotNull = false, Length = 32)]
    public virtual string GivenName { get; set; }

    /// <summary>出生日期</summary>
    [Property(Name = nameof(DateOfBirth), Column = "date_of_birth", Type = "string", NotNull = false, Length = 32)]
    public virtual string DateOfBirth { get; set; }

    /// <summary>性别</summary>
    [Property(Name = nameof(Gender), Column = "gender", Type = "string", NotNull = false, Length = 16)]
    public virtual string Gender { get; set; }

    /// <summary>地址</summary>
    [Property(Name = nameof(StreetAddress), Column = "street_address", Type = "string", NotNull = false, Length = 256)]
    public virtual string StreetAddress { get; set; }

}
