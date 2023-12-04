using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

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

        var compositeFileProvider = new CompositeFileProvider(fileProviders);
        services.AddSingleton<IFileProvider>(compositeFileProvider);

        services.ConfigureSpaFailback(config.GetSection("spaFailback"));
        if (env.IsProduction()) {
            services.ConfigureGzipStatic();
        }

        #if DEBUG
        services.AddDirectoryBrowser();
        #endif
        services.AddSingleton<IContentTypeProvider>(new FileExtensionContentTypeProvider());
    }

    private void ConfigureStaticFiles(WebApplication app, IWebHostEnvironment env) {
        app.UseDefaultFiles();
        if (env.IsProduction()) {
            app.UseGzipStatic();
        }
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
