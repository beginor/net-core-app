using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.DependencyInjection;

namespace Beginor.GisHub.Entry;

partial class Startup {

    private void ConfigureAppServices(IServiceCollection services, IWebHostEnvironment env) {
        var commonOption = new Beginor.GisHub.Common.CommonOption();
        var section = config.GetSection("common");
        section.Bind(commonOption);
        services.AddSingleton(commonOption);
        var cacheFolder = Path.Combine(env.ContentRootPath, commonOption.Cache.Directory);
        if (!Directory.Exists(cacheFolder)) {
            logger.Error($"Cache directory {cacheFolder} does not exists, make sure your config is correct!");
            Directory.CreateDirectory(cacheFolder);
        }
        services.AddDistributedMemoryCache();
        services.AddSingleton<Beginor.GisHub.Common.IFileCacheProvider, Beginor.GisHub.Common.FileCacheProvider>();
        services.AddServiceWithDefaultImplements(
            typeof(Beginor.GisHub.Data.ModelMapping).Assembly,
            t => t.Name.EndsWith("Repository"),
            ServiceLifetime.Scoped
        );
        services.AddScoped<Beginor.GisHub.Common.IRolesFilterProvider, Beginor.GisHub.Data.ResourceRolesFilterProvider>();
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
        services.AddServiceWithDefaultImplements(
            typeof(Beginor.GisHub.DynamicSql.ModelMapping).Assembly,
            t => t.Name.EndsWith("Repository"),
            ServiceLifetime.Scoped
        );
        services.AddSingleton<Beginor.GisHub.DataServices.IDataServiceFactory, Beginor.GisHub.DataServices.DataServiceFactory>();
        services.AddSingleton<Beginor.GisHub.DataServices.JsonSerializerOptionsFactory>();
        services.AddScoped<Beginor.GisHub.DataServices.MarkdownServiceDocBuilder>();
        services.AddScoped<Beginor.GisHub.DataServices.JsonServiceDocBuilder>();
        services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISMetaDataProvider>();
        services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISDataServiceReader>();
        services.AddScoped<Beginor.GisHub.DataServices.PostGIS.PostGISFeatureProvider>();
        services.AddScoped<Beginor.GisHub.DataServices.MySql.MySqlMetaDataProvider>();
        services.AddScoped<Beginor.GisHub.DataServices.MySql.MySqlDataServiceReader>();
        services.AddScoped<Beginor.GisHub.DataServices.MySql.MySqlFeatureProvider>();
        services.AddSingleton<Beginor.GisHub.DynamicSql.IDynamicSqlProvider, Beginor.GisHub.DynamicSql.SmartSqlProvider>();
        services.AddSingleton<Beginor.GisHub.DynamicSql.ParameterConverterFactory>();
    }

    private void ConfigureApp(WebApplication app, IWebHostEnvironment env) {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}
