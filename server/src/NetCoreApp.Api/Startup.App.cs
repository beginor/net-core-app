using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private static void ConfigureAppServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            ModelMapping.Setup();
            // services.AddScoped<IRoleService, RoleService>();
        }

        private static void ConfigureApp(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing now.
        }
    }

}
