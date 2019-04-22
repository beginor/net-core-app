using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureIdentityServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            services.AddDefaultIdentity<AppUser>()
                .AddRoles<AppRole>()
                .AddHibernateStores();
        }

        private void ConfigureIdentity(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing now
        }

    }

}
