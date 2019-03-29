using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureCookiePolicyServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            services.Configure<CookiePolicyOptions>(opts => {
                opts.CheckConsentNeeded = context => true;
                opts.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        private void ConfigureCookiePolicy(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseCookiePolicy();
        }

    }

}
