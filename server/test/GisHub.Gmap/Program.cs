using System.IO;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;
using Beginor.GisHub.Gmap.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Beginor.GisHub.Gmap {

    public class Program {

        public static void Main(string[] args) {
            System.ComponentModel.TypeDescriptor.AddAttributes(
                typeof(System.Net.IPAddress),
                new System.ComponentModel.TypeConverterAttribute(typeof(IPAddressConverter))
            );
            var builder = WebApplication.CreateBuilder(args);
            // logging
            builder.Logging.ClearProviders().AddLog4net(Path.Combine("config", "log.config"));
            // add configuration
            builder.Configuration.AddJsonFile(Path.Combine("config", "appsettings.json"), true, true);
            builder.Configuration.AddJsonFile(Path.Combine("config", $"appsettings.{builder.Environment.EnvironmentName}.json"), true, true);
            // config services;
            var configuration = builder.Configuration;
            builder.Services.Configure<KestrelServerOptions>(configuration.GetSection("kestrel"));
            builder.Services.Configure<EBusOptions>(configuration.GetSection("ebus"));
            builder.Services.AddSingleton<YztService>();
            builder.Services.Configure<ApiProxyOptions>(configuration.GetSection("apiProxy"));
            builder.Services.Configure<ForwardedHeadersOptions>(configuration.GetSection("forwardedHeaders"));
            builder.Services.AddCors(cors => cors.AddDefaultPolicy(configuration.GetSection("cors").Get<CorsPolicy>()));
            builder.Services.AddControllers();
            // build and config app
            var app = builder.Build();
            app.UsePathBase("/gmap");
            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.UseCors();
            app.Map("/ebus", subApp => {
                subApp.UseMiddleware<EBusMiddleware>();
            });
            app.Map("/proxy", subApp => {
                subApp.UseMiddleware<ApiProxyMiddleware>();
            });
            app.UseRouting(); // tmp fix because pathbase is ignored.
            app.MapControllers();
            // run the app
            app.Run();
        }
    }

}
