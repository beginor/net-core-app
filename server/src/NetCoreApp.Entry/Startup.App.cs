using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureAppServices(IServiceCollection services, IWebHostEnvironment env) {
        var commonOption = new Beginor.NetCoreApp.Common.CommonOption();
        var section = config.GetSection("common");
        section.Bind(commonOption);
        services.AddSingleton(commonOption);
        var cacheFolder = Path.Combine(env.ContentRootPath, commonOption.Cache.Directory);
        if (!Directory.Exists(cacheFolder)) {
            logger.Error($"Cache directory {cacheFolder} does not exists, make sure your config is correct!");
            Directory.CreateDirectory(cacheFolder);
        }
        services.AddDistributedMemoryCache();
        services.AddServiceWithDefaultImplements(
            typeof(Beginor.NetCoreApp.Data.ModelMapping).Assembly,
            t => t.Name.EndsWith("Repository"),
            ServiceLifetime.Scoped
        );
    }

    private static void ConfigureApp(WebApplication app, IWebHostEnvironment env) {
        // do nothing now.
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}
