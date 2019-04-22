using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.NetCore;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureHibernateServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
            var cfg = new Configuration();
            var configFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                "hibernate.config"
            );
            cfg.Configure(configFile);
            var isNotProd = !env.IsProduction();
            cfg.SetProperty(
                NHibernate.Cfg.Environment.ShowSql,
                isNotProd.ToString()
            );
            cfg.SetProperty(
                NHibernate.Cfg.Environment.FormatSql,
                isNotProd.ToString()
            );
            cfg.AddIdentityMappingsForPostgres();
            services.AddHibernate(cfg);
        }

        private void ConfigureHibernate(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing know
        }

    }

}
