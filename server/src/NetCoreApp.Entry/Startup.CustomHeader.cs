using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry; 

partial class Startup {

    private void ConfigureCustomHeaderServices(IServiceCollection services, IWebHostEnvironment env) {
        services.ConfigureCustomHeader(config.GetSection("customHeader"));
    }

    private void ConfigureCustomHeader(WebApplication app, IWebHostEnvironment env) {
        app.UseCustomHeader();
    }
}