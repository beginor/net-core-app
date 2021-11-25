using Beginor.GisHub.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureMiddlewareServices(IServiceCollection services, IWebHostEnvironment env) {
            // do nothing now.
        }

        private void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env) {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<AuditLogMiddleware>();
        }
    }
}
