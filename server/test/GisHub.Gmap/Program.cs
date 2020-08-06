using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;

namespace Gmap {

    public class Program {

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) => {
                    var env = context.HostingEnvironment;
                    config
                        .AddJsonFile(Path.Combine("config", "appsettings.json"), true, true)
                        .AddJsonFile(Path.Combine("config", $"appsettings.{env.EnvironmentName}.json"), true, true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((context, services) => {
                    services.Configure<KestrelServerOptions>(
                        context.Configuration.GetSection("kestrel")
                    );
                })
                .ConfigureLogging((context, logging) => {
                    logging.ClearProviders();
                    logging.AddLog4net(Path.Combine("config", "log.config"));
                })
                .ConfigureWebHostDefaults(app => {
                    app.UseStartup<Startup>();
                });
    }
}
