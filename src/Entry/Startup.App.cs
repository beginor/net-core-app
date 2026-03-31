using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureAppServices(IServiceCollection services, IWebHostEnvironment env) {
        services.AddDistributedMemoryCache();
        services.AddCommon(config, env);
        services.AddData();
    }

    private static void ConfigureApp(WebApplication app, IWebHostEnvironment env) {
        // do nothing now.
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}
