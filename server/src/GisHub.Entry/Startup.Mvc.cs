using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureMvcServices(IServiceCollection services, IWebHostEnvironment env) {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager => {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.Api.Controllers.AccountController).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.Slpk.ModelMapping).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.TileMap.ModelMapping).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.DataServices.ModelMapping).Assembly)
                    );
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Beginor.GisHub.DynamicSql.ModelMapping).Assembly)
                    );
                })
                .AddControllersAsServices()
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressConsumesConstraintForFormFileParameters = false;
                    options.SuppressInferBindingSourcesForParameters = false;
                    options.SuppressModelStateInvalidFilter = false;
                });
        }

        private void ConfigureMvc(WebApplication app, IWebHostEnvironment env) {
            app.MapControllers();
        }
    }

}
