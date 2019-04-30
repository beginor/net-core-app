using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureMvcServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            services.AddMvc()
                .ConfigureApplicationPartManager(manager => {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Startup).Assembly)
                    );
                })
                .AddControllersAsServices()
                .ConfigureApiBehaviorOptions(opts => {
                    opts.SuppressConsumesConstraintForFormFileParameters = false;
                    opts.SuppressInferBindingSourcesForParameters = false;
                    opts.SuppressModelStateInvalidFilter = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private void ConfigureMvc(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }

}
