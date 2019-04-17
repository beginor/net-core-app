using System;
using Beginor.NetCoreApp.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beginor.NetCoreApp.Test {

    public abstract class BaseTest {

        protected static IServiceProvider ServiceProvider { get; private set; }

        protected BaseTest() {
            if (ServiceProvider != null) {
                return;
            }
            var config = new ConfigurationBuilder()
//                .AddJsonFile("appsettings.json", true, true)
//                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();
            IHostingEnvironment env = new TestHostingEnvironment();
            var services = new ServiceCollection();
            var startup = new Startup(config, env);
            startup.ConfigureServices(services);
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

}
