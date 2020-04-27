using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.NetCoreApp.Api.Middlewares;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        [System.Diagnostics.Conditional("DEBUG")]
        private void ConfigureStaticFilesServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddSpaFailback(options => {
                var section = config.GetSection("spaFailback");
                section.Bind(options);
            });
#if DEBUG
            services.AddDirectoryBrowser();
#endif
        }

        private void ConfigureStaticFiles(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseDefaultFiles();
            app.UseSpaFailback();
            app.UseStaticFiles();
#if DEBUG
            app.UseDirectoryBrowser();
#endif
        }

    }
}
