using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private static void ConfigureAutoMapperServices(IServiceCollection services, IWebHostEnvironment env) {
        services.AddAutoMapper(
            configure => {
                configure.LicenseKey = null;
                configure.AllowNullCollections = false;
                configure.AllowNullDestinationValues = false;
            },
            [
                typeof(Beginor.NetCoreApp.Data.ModelMapping).Assembly,
            ],
            ServiceLifetime.Singleton
        );
    }

    private static void ConfigureAutoMapper(WebApplication app, IWebHostEnvironment env) {
        // do nothing now.
    }
}
