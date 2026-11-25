using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureOpenApiServices(IServiceCollection services, IWebHostEnvironment env) {
        logger.Debug("Start add openapi related services...");
        // default document is "v1"
        services.AddOpenApi();
        logger.Debug("Add openapi related service completed.");
    }

    private void ConfigureOpenApi(WebApplication app, IWebHostEnvironment env) {
        logger.Debug("Start map openapi ...");
        // default openapi url /openapi/v1.json
        app.MapOpenApi().RequireAuthorization("openapi.read");
        logger.Debug("Map openapi completed!");
    }

}
