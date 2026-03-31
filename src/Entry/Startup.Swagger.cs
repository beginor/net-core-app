using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureSwaggerServices(IServiceCollection services, IWebHostEnvironment env) {
        logger.Debug("Start add opan api related services...");
        if (env.IsDevelopment()) {
            services.AddOpenApi();
        }
        logger.Debug("Add opan api related service completed.");
    }

    private void ConfigureSwagger(WebApplication app, IWebHostEnvironment env) {
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }
    }

}
