using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.NetCore;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureHibernateServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var cfg = new Configuration();
            var configFile = Path.Combine(
                Directory.GetCurrentDirectory(),
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
            cfg.AddIdentityMappingsForPostgres();
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
