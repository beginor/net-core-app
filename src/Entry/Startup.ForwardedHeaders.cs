using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Entry;

partial class Startup {

    private void ConfigureForwardedHeadersServices(IServiceCollection services, IWebHostEnvironment env) {
        var section = config.GetSection("forwardedHeaders");
        services.Configure<ForwardedHeadersOptions>(section);
    }

    private void ConfigureForwardedHeaders(WebApplication app, IWebHostEnvironment env) {
        var options = app.Configuration.GetSection("forwardedHeaders").Get<ForwardedHeadersOptions>();
        var opts = app.Services.GetService<IOptions<ForwardedHeadersOptions>>();
        app.UseForwardedHeaders();
    }
}
