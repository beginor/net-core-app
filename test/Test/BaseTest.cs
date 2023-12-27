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

    protected static IServiceProvider ServiceProvider { get; private set; }

    protected BaseTest() {
        if (ServiceProvider != null) {
            return;
        }
        var services = new ServiceCollection();
        // setup test hosting env
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        IWebHostEnvironment env = new TestHostEnvironment();
        env.ContentRootPath = Path.Combine(baseDir);
        env.WebRootPath = Path.Combine(env.ContentRootPath, "..", "..", "..", "..", "..", "..", "client", "dist");
        services.AddSingleton(env);
        // config files in config folder;
        var configDir = Path.Combine(env.ContentRootPath, "config");
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(configDir, "appsettings.json"))
            .AddJsonFile(
                Path.Combine(configDir, "appsettings.Development.json")
            )
            .Build();
        services.AddSingleton<IConfiguration>(config);
        // startup and build services;
        var startup = new Startup(config, env);
        services.AddLogging(logging => {
            logging.AddLog4net(Path.Combine(configDir, "log.config"));
        });
        startup.ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider(false);
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
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
            new Claim(Consts.OrganizeUnitIdClaimType, user.OrganizeUnit.Id.ToString()),
            new Claim(Consts.OrganizeUnitCodeClaimType, user.OrganizeUnit.Code)
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

    public string ApplicationName { get; set; }

    public string WebRootPath { get; set; }

    public IFileProvider WebRootFileProvider { get; set; }

    public string ContentRootPath { get; set; }

    public IFileProvider ContentRootFileProvider { get; set; }

}

[TestFixture]
public class HostingEnvironmentTest {

    [Test]
    public void TestEnvironment() {
        var target = new TestHostEnvironment();
        Assert.That(target.IsProduction(), Is.False);
        Assert.That(target.IsDevelopment());
    }

}
