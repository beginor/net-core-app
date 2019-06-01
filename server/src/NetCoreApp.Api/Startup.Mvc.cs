using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureMvcServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            services
                .AddRouting(options => {
                    options.LowercaseUrls = true;
                })
                .AddMvc(options => {
                    if (env.IsProduction()) {
                        options.Filters.Add(
                            new AutoValidateAntiforgeryTokenAttribute()
                        );
                    }
                })
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
            // app.UseMiddleware<Beginor.NetCoreApp.Api.Middlewares.AuditMiddleware>();
            app.UseEndpointRouting().UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }

}
