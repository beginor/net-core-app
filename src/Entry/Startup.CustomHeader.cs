using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureCustomHeaderServices(IServiceCollection services, IWebHostEnvironment env) {
        if (config.GetSection("customHeader").Exists()) {
            services.ConfigureCustomHeader(config.GetSection("customHeader"));
        }
    }

    private void ConfigureCustomHeader(WebApplication app, IWebHostEnvironment env) {
        if (config.GetSection("customHeader").Exists()) {
            app.UseCustomHeader();
        }
    }
}
