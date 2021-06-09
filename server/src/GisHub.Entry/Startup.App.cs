using Beginor.AppFx.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureAppServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddDistributedMemoryCache();
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
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.GisHub.TileMap.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            services.AddServiceWithDefaultImplements(
                typeof(Beginor.GisHub.DataServices.ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );

            var coordinateConverter = new Beginor.GisHub.DataServices.CoordinateConverter();
            var coordinateSection = config.GetSection("coordinate");
            coordinateSection.Bind(coordinateConverter);
            services.AddSingleton<Beginor.GisHub.DataServices.CoordinateConverter>(coordinateConverter);
            services.AddSingleton<Beginor.GisHub.DataServices.IDataServiceFactory, Beginor.GisHub.DataServices.DataServiceFactory>();
            services.AddSingleton<Beginor.GisHub.DataServices.JsonSerializerOptionsFactory>();
            services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISMetaDataProvider>();
            services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGisDataServiceReader>();
            services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISFeatureProvider>();
        }

        private void ConfigureApp(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing now.
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
    }

}
