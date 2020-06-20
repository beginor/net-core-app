using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.Slpk;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureSlpkServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddSlpk(options => {
                config.GetSection("slpk").Bind(options);
            });
        }

        private void ConfigureSlpk(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseSlpk();
        }
    }

}
