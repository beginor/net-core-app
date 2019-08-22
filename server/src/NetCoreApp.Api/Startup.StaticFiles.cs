using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureStaticFilesServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            // do nothing now!
        }

        private void ConfigureStaticFiles(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

    }
}
