using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureStaticFilesServices(IServiceCollection services, IWebHostEnvironment env) {
        IList<IFileProvider> fileProviders = new List<IFileProvider>();
        var rootPath = env.ContentRootPath;
        var wwwroot = Path.Combine(rootPath, "wwwroot");
        if (!Directory.Exists(wwwroot)) {
            Directory.CreateDirectory(wwwroot);
        }
        var wwwrootFileProvider = new PhysicalFileProvider(wwwroot);
        fileProviders.Add(wwwrootFileProvider);

        #if DEBUG
        var clientDist = Path.Combine(rootPath, "../../../client/dist/");
        if (Directory.Exists(clientDist)) {
            var clientDistFileProvider = new PhysicalFileProvider(clientDist);
            fileProviders.Add(clientDistFileProvider);
        }
        #endif

        var compositeFileProvider = new CompositeFileProvider(fileProviders);
        services.AddSingleton<IFileProvider>(compositeFileProvider);
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
