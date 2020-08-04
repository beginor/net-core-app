using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.Api.Middlewares;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureCustomHeaderServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.Configure<CustomHeaderOptions>(
                config.GetSection("customHeader")
            );
        }

        private void ConfigureCustomHeader(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseCustomHeader();
        }
    }

}
