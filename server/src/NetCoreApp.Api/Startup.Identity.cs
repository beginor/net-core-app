using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureIdentityServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            var section = this.config.GetSection("identityOptions");
            var settings = section.Get<IdentityOptions>();
            services
                .Configure<IdentityOptions>(options => {
                    // user settings;
                    options.User.RequireUniqueEmail = settings.User.RequireUniqueEmail;
                    // password settings;
                    options.Password.RequireDigit = settings.Password.RequireDigit;
                    options.Password.RequireLowercase = settings.Password.RequireLowercase;
                    options.Password.RequireNonAlphanumeric = settings.Password.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = settings.Password.RequireUppercase;
                    options.Password.RequiredLength = settings.Password.RequiredLength;
                    options.Password.RequiredUniqueChars = settings.Password.RequiredUniqueChars;
                    // lockout settings;
                    options.Lockout.AllowedForNewUsers = settings.Lockout.AllowedForNewUsers;
                    options.Lockout.DefaultLockoutTimeSpan = settings.Lockout.DefaultLockoutTimeSpan;
                    options.Lockout.MaxFailedAccessAttempts = settings.Lockout.MaxFailedAccessAttempts;
                })
                .AddDefaultIdentity<AppUser>()
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
