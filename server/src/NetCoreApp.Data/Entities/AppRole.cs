using System;
using NHIdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;

namespace Beginor.NetCoreApp.Data.Entities {

    public class AppRole : NHIdentityRole {

        public virtual string Description { get; set; }

        public virtual bool IsDefault { get; set; }

    }

}
