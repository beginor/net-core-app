using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureAuthenticationServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            var section = config.GetSection("cookieAuthOptions");
            var settings = section.Get<CookieAuthenticationOptions>();
            services.AddAuthentication()
                .AddCookie(options => {
                    options.SlidingExpiration = settings.SlidingExpiration;
                    options.ExpireTimeSpan = settings.ExpireTimeSpan;
                });
        }

        private void ConfigureAuthentication(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseAuthentication();
        }

    }

}
