using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private static void ConfigureAutoMapperServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var mapperConfig = new MapperConfiguration(configure => {
                configure.AddMaps(
                    "Beginor.GisHub.Data",
                    "Beginor.GisHub.Slpk"
                );
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void ConfigureAutoMapper(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing now.
        }
    }

}
