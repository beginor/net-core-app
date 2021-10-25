using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.DependencyInjection;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private void ConfigureAppServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var commonOption = new Beginor.NetCoreApp.Common.CommonOption();
            var section = config.GetSection("common");
            section.Bind(commonOption);
            services.AddSingleton(commonOption);
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
