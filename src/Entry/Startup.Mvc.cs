using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureMvcServices(IServiceCollection services, IWebHostEnvironment env) {
        services.AddControllers()
            .ConfigureApplicationPartManager(manager => {
                manager.ApplicationParts.Clear();
                manager.ApplicationParts.Add(
                    new AssemblyPart(typeof(Beginor.NetCoreApp.Api.Controllers.AccountController).Assembly)
                );
                manager.ApplicationParts.Add(
                    new AssemblyPart(typeof(Beginor.NetCoreApp.WeChat.Api.WeChatController).Assembly)
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
