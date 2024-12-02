using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureForwardedHeadersServices(IServiceCollection services, IWebHostEnvironment env) {
        var section = config.GetSection("forwardedHeaders");
        if (section.Exists()) {
            services.Configure<ForwardedHeadersOptions>(section);
        }
    }

    private void ConfigureForwardedHeaders(WebApplication app, IWebHostEnvironment env) {
        if (config.GetSection("forwardedHeaders").Exists()) {
            app.UseForwardedHeaders();
        }
    }
}
