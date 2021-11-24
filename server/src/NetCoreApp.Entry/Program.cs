using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;

namespace Beginor.NetCoreApp.Entry {

    public class Program {

        public static void Main(string[] args) {
            var options = new WebApplicationOptions {
                #if DEBUG
                WebRootPath = "../../../client/dist/"
                #endif
            };
            var builder = WebApplication.CreateBuilder(options);
            var env = builder.Environment;
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine("config", "appsettings.json"), true, true)
                .AddJsonFile(Path.Combine("config", $"appsettings.{env.EnvironmentName}.json"), true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            builder.Logging
                .ClearProviders()
                .AddLog4net(Path.Combine("config", "log.config"));
            var section = builder.Configuration.GetSection("kestrel");
            if (section.Exists()) {
                builder.Services.Configure<KestrelServerOptions>(section);
            }
            var startup = new Startup(builder.Configuration, env);
            startup.ConfigureServices(builder.Services);
            var app = builder.Build();
            startup.Configure(app);
            app.Run();
        }

    }
}
