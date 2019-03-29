using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureAuthenticationServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            services.AddAuthentication()
                .AddCookie();
        }

        private void ConfigureAuthentication(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseAuthentication();
        }

    }

}
