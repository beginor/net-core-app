using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureMvcServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddControllers(options => {
                    // options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();        
                })
                .ConfigureApplicationPartManager(manager => {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Startup).Assembly)
                    );
                })
                .AddControllersAsServices()
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressConsumesConstraintForFormFileParameters = false;
                    options.SuppressInferBindingSourcesForParameters = false;
                    options.SuppressModelStateInvalidFilter = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();
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
