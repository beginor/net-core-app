using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureStaticFilesServices(IServiceCollection services, IWebHostEnvironment env) {
        #if DEBUG
        var rootPath = env.ContentRootPath;
        var fileProvider = new CompositeFileProvider(
            new PhysicalFileProvider(Path.Combine(rootPath, "wwwroot")),
            new PhysicalFileProvider(Path.Combine(rootPath, "../../../client/dist/"))
        );
        services.AddSingleton<IFileProvider>(fileProvider);
        #endif
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
        var fileProvider = app.Services.GetService<IFileProvider>();
        if (fileProvider != null) {
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = fileProvider
            });
        }
        else {
            app.UseStaticFiles();
        }
        #if DEBUG
        app.UseDirectoryBrowser();
        #endif
    }

}
