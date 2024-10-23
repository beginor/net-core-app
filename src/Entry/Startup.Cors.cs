using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.NetCoreApp.Api.Cors;
using Beginor.NetCoreApp.Api.Middlewares;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureCorsServices( IServiceCollection services, IWebHostEnvironment env) {
        var section = config.GetSection("cors");
        var corsPolicy = section.Get<CorsPolicy>();
        services.Configure<CorsPolicy>(section);
        services.AddScoped<ICorsPolicyProvider, CorsPolicyProvider>();
        services.AddCors(options => {
            options.AddDefaultPolicy(corsPolicy!);
        });
    }

    private void ConfigureCors(WebApplication app, IWebHostEnvironment env) {
        var corsEnv = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENABLE_CORS");
        if (!string.IsNullOrEmpty(corsEnv) && bool.TryParse(corsEnv, out var enableCors) && enableCors) {
            app.UseCors();
            app.UseRefererFiltering();
        }
    }

}
