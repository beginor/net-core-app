using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;

namespace Beginor.NetCoreApp.Entry;

public class Program {

    public static void Main(string[] args) {
        AddGlobalConverters();
        var builder = WebApplication.CreateBuilder();
        var env = builder.Environment;
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine("config", "appsettings.json"), true, true)
            .AddJsonFile(Path.Combine("config", $"appsettings.{env.EnvironmentName}.json"), true, true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
        var logConfigFile = Path.Combine("config", "log.config");
        builder.Logging
            .ClearProviders()
            .AddLog4netWithDatabaseConnection(logConfigFile);
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

    private static void AddGlobalConverters() {
        System.ComponentModel.TypeDescriptor.AddAttributes(
            typeof(System.Net.IPAddress),
            new System.ComponentModel.TypeConverterAttribute(typeof(IPAddressConverter))
        );
    }

}
