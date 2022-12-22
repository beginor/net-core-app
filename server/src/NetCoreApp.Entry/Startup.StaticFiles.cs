using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureStaticFilesServices(IServiceCollection services, IWebHostEnvironment env) {
        services.ConfigureSpaFailback(config.GetSection("spaFailback"));
        services.ConfigureGzipStatic();
        #if DEBUG
        services.AddDirectoryBrowser();
        #endif
    }

    private void ConfigureStaticFiles(WebApplication app, IWebHostEnvironment env) {
        app.UseDefaultFiles();
        app.UseGzipStatic();
        app.UseSpaFailback();
        app.UseStaticFiles();
        #if DEBUG
        app.UseDirectoryBrowser();
        #endif
    }

}
