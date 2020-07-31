using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                .ConfigureLogging((context, logging) => {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(app => {
                    app.UseStartup<Startup>();
                });
    }
}
