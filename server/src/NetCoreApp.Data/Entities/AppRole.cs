using System;
using NHibernate.Mapping.Attributes;
using NHIdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;

namespace Beginor.NetCoreApp.Data.Entities;

#nullable disable

[JoinedSubclass(0, Schema = "public", Table = "app_roles", ExtendsType = typeof(NHIdentityRole))]
[Key(1, Column = "id")]
public class AppRole : NHIdentityRole {

    [Property(Column = "description", Type = "string", NotNull = false, Length = 256)]
    public virtual string Description { get; set; }

    [Property(Column = "is_default", Type = "bool", NotNull = true)]
    public virtual bool IsDefault { get; set; }

    [Property(Column = "is_anonymous", Type = "bool", NotNull = true)]
    public virtual bool IsAnonymous { get; set; }

}
