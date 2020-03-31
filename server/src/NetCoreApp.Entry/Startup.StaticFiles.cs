using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        [System.Diagnostics.Conditional("DEBUG")]
        private void ConfigureStaticFilesServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            // do nothing now!
            services.AddDirectoryBrowser();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void ConfigureStaticFiles(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseDirectoryBrowser();
        }

    }
}
