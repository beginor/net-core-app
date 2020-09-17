using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.NetCore;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureHibernateServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var cfg = new Configuration();
            var configFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                "config",
                "hibernate.config"
            );
            cfg.Configure(configFile);
            var isTest = env.IsEnvironment("Test");
            cfg.SetProperty(
                Environment.ShowSql,
                isTest.ToString()
            );
            cfg.SetProperty(
                Environment.FormatSql,
                isTest.ToString()
            );
            cfg.AddIdentityMappings();
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.Data.Entities.AppUser).Assembly);
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.Slpk.Data.SlpkEntity).Assembly);
            services.AddHibernate(cfg);
        }

        private void ConfigureHibernate(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing know
        }

    }

    public static class ConfigurationExtensions {

        public static Configuration AddAttributeMappingAssembly(this Configuration cfg, Assembly assembly) {
            HbmSerializer.Default.Validate = true;
            var stream = HbmSerializer.Default.Serialize(assembly);
            using var reader = new StreamReader(stream);
            var xml = reader.ReadToEnd();
            cfg.AddXml(xml);
            return cfg;
        }

    }

}
