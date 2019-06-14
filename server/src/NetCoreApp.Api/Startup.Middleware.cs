using Beginor.NetCoreApp.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureMiddlewareServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            // do nothing now.
        }

        private void ConfigureMiddleware(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseMiddleware<AuditMiddleware>();
        }
    }
}
