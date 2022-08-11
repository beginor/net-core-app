using System;
using Beginor.AppFx.Core;
using NHibernate.Mapping.Attributes;
using NHIdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

namespace Beginor.NetCoreApp.Data.Entities; 

[JoinedSubclass(0, Schema = "public", Table = "app_users", ExtendsType = typeof(NHIdentityUser))]
[Key(1, Column = "id")]
public class AppUser : NHIdentityUser {

    [Property(Column = "create_time", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = true, Generated = PropertyGeneration.Insert, Update = false, Insert = false)]
    public virtual DateTime CreateTime { get; set; }

    [Property(Column = "last_login", TypeType = typeof(NHibernate.Extensions.NpgSql.TimeStampType), NotNull = false)]
    public virtual DateTime? LastLogin { get; set; }

    [Property(Column = "login_count", Type = "int", NotNull = true)]
    public virtual int LoginCount { get; set; }

}
