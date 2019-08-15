using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private static void ConfigureAutoMapperServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            var mapperConfig = new MapperConfiguration(configure => {
                configure.AddMaps(
                    "Beginor.NetCoreApp.Services"
                );
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton<IMapper>(mapper);
        }

        private static void ConfigureAutoMapper(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing now.
        }
    }

}
