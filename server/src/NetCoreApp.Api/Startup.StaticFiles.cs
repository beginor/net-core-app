using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
            if (env.IsDevelopment()) {
                // find client dist directory;
                var currDir = Directory.GetCurrentDirectory();
                var clientDistDir = Path.Combine(
                    currDir,
                    "../../../client/dist/"
                );
                // static file options;
                var opts = new StaticFileOptions {
                    FileProvider = new PhysicalFileProvider(clientDistDir)
                };
                app.UseStaticFiles(opts);
                app.UseDefaultFiles();
            }
            else {
                app.UseStaticFiles();
                app.UseDefaultFiles();
            }
        }

    }
}
