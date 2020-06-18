using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.Api.Middlewares;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureCustomHeaderServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddCustomHeader(options => {
                var section = config.GetSection("customHeader");
                section.Bind(options);
            });
        }

        private void ConfigureCustomHeader(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseCustomHeader();
        }
    }

}
