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
#if DEBUG
            services.AddDirectoryBrowser();
#endif
        }

        private void ConfigureStaticFiles(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseDefaultFiles();
            app.UseStaticFiles();
#if DEBUG
            app.UseDirectoryBrowser();
#endif
        }

    }
}
