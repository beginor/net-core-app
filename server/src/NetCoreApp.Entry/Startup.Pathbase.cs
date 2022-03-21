using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry; 

partial class Startup {

    private void ConfigurePathBaseServices(IServiceCollection services, IWebHostEnvironment env) {
        // do nothing now.
    }

    private void ConfigurePathBase(WebApplication app, IWebHostEnvironment env) {
        var pathbase = GetAppPathbase();
        if (string.IsNullOrEmpty(pathbase)) {
            return;
        }
        app.UsePathBase(new PathString(pathbase));
        var message = "Hosting pathbase: " + pathbase;
        logger.Info(message);
    }

}