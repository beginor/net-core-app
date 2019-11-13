using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private static readonly string CorsPolicyName = "defaultCorsPolicy";

        private void ConfigureCorsServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddCors(options => {
                var section = config.GetSection("cors");
                var settings = section.Get<CorsPolicy>();
                options.AddPolicy(CorsPolicyName, settings);
            });
        }

        private void ConfigureCors(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseCors(CorsPolicyName);
        }

    }

}
