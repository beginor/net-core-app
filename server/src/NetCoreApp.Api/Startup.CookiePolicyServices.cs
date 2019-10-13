using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureCookiePolicyServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.Configure<CookiePolicyOptions>(opts => {
                opts.CheckConsentNeeded = context => true;
                opts.MinimumSameSitePolicy = SameSiteMode.Lax;
            });
            services.AddAntiforgery(options => {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = true;
            });
        }

        private void ConfigureCookiePolicy(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseCookiePolicy();
        }

    }

}
