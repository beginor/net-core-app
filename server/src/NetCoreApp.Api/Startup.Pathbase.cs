using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigurePathBaseServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            // do nothing now.
        }

        private void ConfigurePathBase(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            var pathbase = GetAppPathbase();
            if (string.IsNullOrEmpty(pathbase)) {
                return;
            }
            app.UsePathBase(new PathString(pathbase));
            var message = "Hosting pathbase: " + pathbase;
            log.Debug(message);
        }

    }

}
