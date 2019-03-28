using System;
using NHIdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;

namespace Beginor.NetCoreApp.Data.Entities {

    public class ApplicationRole : NHIdentityRole {

        public virtual string Description { get; set; }

    }

}
