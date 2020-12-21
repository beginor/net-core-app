using System;
using System.Collections.Concurrent;
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
                typeof(Beginor.GisHub.Data.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.GisHub.Slpk.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddSingleton<ConcurrentDictionary<long, Beginor.GisHub.Slpk.Data.SlpkCacheItem>>();
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.GisHub.TileMap.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddSingleton<ConcurrentDictionary<long, Beginor.GisHub.TileMap.Data.TileMapCacheItem>>();
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.GisHub.DataServices.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddSingleton<Beginor.GisHub.DataServices.IDataServiceFactory, Beginor.GisHub.DataServices.DataServiceFactory>();
            services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISMetaDataProvider>();
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
