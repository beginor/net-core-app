using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;


namespace Beginor.NetCoreApp.Entry {

    public class Program {

        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) => {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    var env = hostContext.HostingEnvironment;
                    config.AddJsonFile(
                        Path.Combine(Directory.GetCurrentDirectory(), "config", "appsettings.json"),
                        true,
                        true
                    ).AddJsonFile(
                        Path.Combine(Directory.GetCurrentDirectory(), "config", $"appsettings.{env.EnvironmentName}.json"),
                        true,
                        true
                    );
                    config.AddEnvironmentVariables();
                    if (args != null) {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "config",
                        "log.config"
                    );
                    logging.AddLog4net(path);
                })
                .ConfigureWebHostDefaults(webHost => {
                    #if DEBUG
                    webHost.UseWebRoot("../../../client/dist/");
                    #endif
                    webHost.UseStartup<Startup>();
                });
        }

    }
}
