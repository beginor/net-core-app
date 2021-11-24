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

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private void ConfigureHibernateServices(IServiceCollection services, IWebHostEnvironment env) {
            var cfg = new Configuration();
            var configFile = Path.Combine("config", "hibernate.config");
            cfg.Configure(configFile);
            var isDevelopment = env.IsDevelopment().ToString();
            cfg.SetProperty(Environment.ShowSql, isDevelopment);
            cfg.SetProperty(Environment.FormatSql, isDevelopment);
            cfg.AddIdentityMappings();
            cfg.AddAttributeMappingAssembly(typeof(Beginor.NetCoreApp.Data.ModelMapping).Assembly);
            services.AddHibernate(cfg);
        }

        private void ConfigureHibernate(WebApplication app, IWebHostEnvironment env) {
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
