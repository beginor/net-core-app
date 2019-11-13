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
            IWebHostEnvironment env
        ) {
            var identitySection = config.GetSection("identity");
            var identitySettings = identitySection.Get<IdentityOptions>();
            services
                .AddIdentity<AppUser, AppRole>(options => {
                    // user settings;
                    options.User.RequireUniqueEmail = identitySettings.User.RequireUniqueEmail;
                    // password settings;
                    options.Password.RequireDigit = identitySettings.Password.RequireDigit;
                    options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
                    options.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;
                    options.Password.RequiredLength = identitySettings.Password.RequiredLength;
                    options.Password.RequiredUniqueChars = identitySettings.Password.RequiredUniqueChars;
                    // lockout settings;
                    options.Lockout.AllowedForNewUsers = identitySettings.Lockout.AllowedForNewUsers;
                    options.Lockout.DefaultLockoutTimeSpan = identitySettings.Lockout.DefaultLockoutTimeSpan;
                    options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAccessAttempts;
                })
                .AddDefaultTokenProviders()
                .AddHibernateStores();
        }

        private void ConfigureIdentity(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing now
        }

    }

}
