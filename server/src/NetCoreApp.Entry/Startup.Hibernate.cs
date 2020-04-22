using System.IO;
using Beginor.NetCoreApp.Data.Entities;
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
            var isNotProd = !env.IsProduction();
            cfg.SetProperty(
                Environment.ShowSql,
                isNotProd.ToString()
            );
            cfg.SetProperty(
                Environment.FormatSql,
                isNotProd.ToString()
            );
            cfg.AddIdentityMappings();
            HbmSerializer.Default.Validate = true;
            var stream = HbmSerializer.Default.Serialize(typeof(AppUser).Assembly);
            using var reader = new StreamReader(stream);
            var xml = reader.ReadToEnd();
            cfg.AddXml(xml);
            services.AddHibernate(cfg);
        }

        private void ConfigureHibernate(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            // do nothing know
        }

    }

}
