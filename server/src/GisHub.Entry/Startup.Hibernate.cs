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
            var isDevelopment = env.IsDevelopment().ToString();
            cfg.SetProperty(Environment.ShowSql, isDevelopment);
            cfg.SetProperty(Environment.FormatSql, isDevelopment);
            cfg.AddIdentityMappings();
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.Data.ModelMapping).Assembly);
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.Slpk.ModelMapping).Assembly);
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.TileMap.ModelMapping).Assembly);
            cfg.AddAttributeMappingAssembly(typeof(Beginor.GisHub.DataServices.ModelMapping).Assembly);
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
