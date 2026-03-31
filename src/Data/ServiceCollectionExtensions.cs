using Microsoft.Extensions.DependencyInjection;
using Beginor.AppFx.DependencyInjection;

namespace Beginor.NetCoreApp.Data;

public static class ServiceCollectionExtensions {

    extension(IServiceCollection services) {

        public IServiceCollection AddData() {
            services.AddServiceWithDefaultImplements(
                typeof(ModelMapping).Assembly,
                t => t.Name.EndsWith("Repository"),
                ServiceLifetime.Scoped
            );
            return services;
        }

    }

}
