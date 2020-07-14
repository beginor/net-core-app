using System;
using System.IO;
using System.Reflection;
using Beginor.AppFx.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private static void ConfigureAppServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            services.AddServiceWithDefaultImplements(
                Assembly.LoadFrom(Path.Combine(baseDir, "Beginor.GisHub.Data.dll")),
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddServiceWithDefaultImplements(
                Assembly.LoadFrom(Path.Combine(baseDir, "Beginor.GisHub.Slpk.dll")),
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddSingleton<Beginor.GisHub.Slpk.Cache.SlpkCache>();
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
