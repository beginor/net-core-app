using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Logging.Log4net;

namespace Beginor.NetCoreApp.Api {

    public class Program {

        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    logging => {
                        logging.ClearProviders();
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "log.config"
                        );
                        logging.AddLog4net(path);
                    }
                )
                #if DEBUG
                .UseWebRoot("../../../client/dist/")
                #endif
                .UseStartup<Startup>();
        }

    }
}
