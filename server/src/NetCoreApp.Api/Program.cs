using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;


namespace Beginor.NetCoreApp.Api {

    public class Program {

        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(),
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
