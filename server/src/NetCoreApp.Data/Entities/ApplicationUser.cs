using System;
using NHIdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

namespace Beginor.NetCoreApp.Data.Entities {

    public class ApplicationUser : NHIdentityUser {

        public virtual DateTime CreateTime { get; set; }

        public virtual DateTime? LastLogin { get; set; }

        public virtual int LoginCount { get; set; }

    }

}
