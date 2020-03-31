using Beginor.NetCoreApp.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private void ConfigureMiddlewareServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            // do nothing now.
        }

        private void ConfigureMiddleware(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<AuditMiddleware>();
        }
    }
}
