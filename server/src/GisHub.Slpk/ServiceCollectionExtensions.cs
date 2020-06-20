using System;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Slpk {

    public static class ServiceCollectionExtensions {

        public static void AddSlpk(
            this IServiceCollection services,
            Action<SlpkOptions> config
        ) {
            var options = new SlpkOptions();
            config(options);
            services.AddSingleton(options);
        }

    }

}
