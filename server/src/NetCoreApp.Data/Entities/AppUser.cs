using System;
using Beginor.AppFx.Core;
using NHIdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

namespace Beginor.NetCoreApp.Data.Entities {

    public class AppUser : NHIdentityUser {

        public virtual DateTime CreateTime { get; set; }

        public virtual DateTime? LastLogin { get; set; }

        public virtual int LoginCount { get; set; }

    }

}
