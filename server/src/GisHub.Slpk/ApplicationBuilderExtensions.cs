using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Slpk {

    public static class ApplicationBuilderExtensions {

        public static void UseSlpk(this IApplicationBuilder app) {
            var options = app.ApplicationServices.GetService<SlpkOptions>();
            var logger = app.ApplicationServices.GetService<ILogger<SlpkMiddleware>>();
            logger.LogInformation(
                $"Slpk: {options.PathBase} => {options.RootFolder}");
            app.Map(options.PathBase, slpkApp => {
                slpkApp.UseMiddleware<SlpkMiddleware>(options);
                slpkApp.UseStaticFiles();
            });
        }

    }

}
