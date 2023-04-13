using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
        #if DEBUG
        var rootPath = env.ContentRootPath;
        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new CompositeFileProvider(
                new PhysicalFileProvider(Path.Combine(rootPath, "wwwroot")),
                new PhysicalFileProvider(Path.Combine(rootPath, "../../../client/dist/"))
            )
        });
        app.UseDirectoryBrowser();
        #else
        app.UseStaticFiles();
        #endif
    }

}
