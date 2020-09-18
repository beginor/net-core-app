using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureMvcServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager => {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.Api.Controllers.AccountController).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.Slpk.Api.SlpkController).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.TileMap.Api.TileMapController).Assembly)
                    );
                })
                .AddControllersAsServices()
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressConsumesConstraintForFormFileParameters = false;
                    options.SuppressInferBindingSourcesForParameters = false;
                    options.SuppressModelStateInvalidFilter = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        private void ConfigureMvc(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseEndpoints(routes => {
                routes.MapControllers();
            });
        }
    }

}
