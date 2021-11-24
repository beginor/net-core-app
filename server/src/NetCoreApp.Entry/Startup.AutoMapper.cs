using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private static void ConfigureAutoMapperServices(IServiceCollection services, IWebHostEnvironment env) {
            var mapperConfig = new MapperConfiguration(configure => {
                configure.AddMaps(
                    typeof(Beginor.NetCoreApp.Data.ModelMapping).Assembly
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
