using System;
using System.IO;
using System.Reflection;
using Beginor.AppFx.DependencyInjection;
using Beginor.NetCoreApp.Data.Repositories;
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
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            services.AddServiceWithDefaultImplements(
                Assembly.LoadFrom(Path.Combine(baseDir, "Beginor.NetCoreApp.Data.dll")),
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddServiceWithDefaultImplements(
                Assembly.LoadFrom(Path.Combine(baseDir, "Beginor.NetCoreApp.Services.dll")),
                t => t.Name.EndsWith("Service"),
                ServiceLifetime.Scoped
            );
        }

        private static void ConfigureApp(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing now.
        }
    }

}
