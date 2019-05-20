using System;
using System.IO;
using Beginor.NetCoreApp.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test {

    public abstract class BaseTest {

        protected static IServiceProvider ServiceProvider { get; private set; }

        protected BaseTest() {
            if (ServiceProvider != null) {
                return;
            }
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(baseDir, "appsettings.json"))
                .AddJsonFile(
                    Path.Combine(baseDir, "appsettings.Development.json")
                )
                .Build();
            IHostingEnvironment env = new TestHostingEnvironment();
            var services = new ServiceCollection();
            var startup = new Startup(config, env);
            startup.ConfigureServices(services);
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton(env);
            ServiceProvider = services.BuildServiceProvider(false);
        }

    }

    public abstract class BaseTest<T> : BaseTest where T : class {

        public T Target => ServiceProvider.GetService(typeof(T)) as T;

    }

    public class TestHostingEnvironment : IHostingEnvironment {

        public string EnvironmentName { get; set; }

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
            var target = new TestHostingEnvironment();
            Assert.IsFalse(target.IsProduction());
            Assert.IsFalse(target.IsDevelopment());
        }

    }

}
