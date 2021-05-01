using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.DependencyInjection;
using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private static void ConfigureAppServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddDistributedMemoryCache();
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.NetCoreApp.Data.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
        }

        private static void ConfigureApp(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing now.
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
    }

}
