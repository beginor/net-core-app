using System;
using System.IO;
using System.Reflection;
using Beginor.AppFx.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private static void ConfigureAppServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            services.AddServiceWithDefaultImplements(
                Assembly.LoadFrom(Path.Combine(baseDir, "Beginor.NetCoreApp.Data.dll")),
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
