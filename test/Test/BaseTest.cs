using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NHibernate;
using NUnit.Framework;
using Beginor.AppFx.Logging.Log4net;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Entry;

namespace Beginor.NetCoreApp.Test;

public abstract class BaseTest {

     protected IServiceProvider ServiceProvider { get; private set; } = null!;
     protected JsonSerializerOptions JsonSerializerOptions { get; private set; } = null!;

    protected BaseTest() {
        this.InitTest();
    }

    private void InitTest() {
        InitDapper();
        this.JsonSerializerOptions = this.InitTestJsonOptions();
        this.ServiceProvider = this.InitServiceProvider();
    }

    protected virtual void InitDapper() {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
     protected virtual IServiceProvider InitServiceProvider() {
         var services = new ServiceCollection();
        // setup test hosting env
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var env = new TestHostEnvironment {
            ContentRootPath = baseDir
        };
        services.AddSingleton<IWebHostEnvironment>(env);
        services.AddSingleton<IHostEnvironment>(env);
        // config files in config folder;
        var configDir = Path.Combine(env.ContentRootPath, "config");
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(configDir, "appsettings.json"))
            .AddJsonFile(
                Path.Combine(configDir, $"appsettings.{env.EnvironmentName}.json")
            )
            .Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddLogging(logging => {
            logging.AddLog4net(Path.Combine(configDir, "log.config"));
        });
        this.InitTestServices(env, services, config);
        return services.BuildServiceProvider(false);
     }

     protected virtual void InitTestServices(
        IWebHostEnvironment environment,
        IServiceCollection services,
        IConfiguration configuration
    ) {
        // startup and build services;
        var startup = new Startup(configuration, environment);
        startup.ConfigureServices(services);
    }

     protected virtual JsonSerializerOptions InitTestJsonOptions() {
        return new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
    }

    protected JsonSerializerOptions GetTestJsonOption() {
        var option = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        return option;
    }

    protected ClaimsPrincipal CreateTestPrincipal() {
        var userName = "admin";
        var session = ServiceProvider.GetService<ISession>();
        // using var session = factory.OpenSession();
        var user = session.Query<AppUser>().First(x => x.UserName == userName);

        var identity = new ClaimsIdentity(new [] {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(AppClaimTypes.OrganizeUnitId, user.OrganizeUnit.Id.ToString()),
            new Claim(AppClaimTypes.OrganizeUnitCode, user.OrganizeUnit.Code)
        }, "TestAuth");

        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

}

public abstract class BaseTest<T> : BaseTest where T : class {

    public T Target => ServiceProvider.GetService(typeof(T)) as T;

}

public class TestHostEnvironment : IWebHostEnvironment {

    public string EnvironmentName { get; set; } = "Development";

    public string ApplicationName { get; set; } = "Beginor.NetCoreApp.Test";

    public string WebRootPath { get; set; }

    public IFileProvider WebRootFileProvider { get; set; }

    public string ContentRootPath { get; set; }

    public IFileProvider ContentRootFileProvider { get; set; }

    public TestHostEnvironment() {
        this.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;
        this.WebRootPath = AppDomain.CurrentDomain.BaseDirectory;
        this.WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
        this.ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
    }
}

[TestFixture]
public class HostingEnvironmentTest {

    [Test]
    public void TestEnvironment() {
        var target = new TestHostEnvironment();
        Assert.That(target.IsProduction(), Is.False);
        Assert.That(target.IsDevelopment(), Is.True);
    }

}
