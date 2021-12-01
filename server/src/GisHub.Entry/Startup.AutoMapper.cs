using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private static void ConfigureAutoMapperServices(IServiceCollection services, IWebHostEnvironment env) {
            var mapperConfig = new MapperConfiguration(configure => {
                configure.AddMaps(
                    typeof(Beginor.GisHub.Data.ModelMapping).Assembly,
                    typeof(Beginor.GisHub.Slpk.ModelMapping).Assembly,
                    typeof(Beginor.GisHub.TileMap.ModelMapping).Assembly,
                    typeof(Beginor.GisHub.DataServices.ModelMapping).Assembly,
                    typeof(Beginor.GisHub.DynamicSql.ModelMapping).Assembly
                );
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void ConfigureAutoMapper(WebApplication app, IWebHostEnvironment env) {
            // do nothing now.
        }
    }

}
