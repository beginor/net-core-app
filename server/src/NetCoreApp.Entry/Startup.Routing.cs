using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry; 

partial class Startup {

    private void ConfigureRoutingServices(IServiceCollection services, IWebHostEnvironment env) {
        services.AddRouting(options => {
            options.LowercaseUrls = true;
        });
    }

    private void ConfigureRouting(WebApplication app, IWebHostEnvironment env) {
        app.UseRouting();
    }

}