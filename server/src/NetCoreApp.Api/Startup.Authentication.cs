using Beginor.NetCoreApp.Api.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureAuthenticationServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            // authentication;
            var section = config.GetSection("cookieAuthOptions");
            var settings = section.Get<CookieAuthenticationOptions>();
            services
                .ConfigureApplicationCookie(options => {
                    options.SlidingExpiration = settings.SlidingExpiration;
                    options.ExpireTimeSpan = settings.ExpireTimeSpan;
                })
                .ConfigureExternalCookie(options => {
                    options.SlidingExpiration = settings.SlidingExpiration;
                    options.ExpireTimeSpan = settings.ExpireTimeSpan;
                });
            // authorization;
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }

        private void ConfigureAuthentication(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseAuthentication();
            app.UseAuthorization();
        }

    }

}
