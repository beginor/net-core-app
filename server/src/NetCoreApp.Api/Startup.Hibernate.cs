using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            cfg.SetProperty(
                NHibernate.Cfg.Environment.ShowSql,
                env.IsDevelopment().ToString()
            );
            cfg.SetProperty(
                NHibernate.Cfg.Environment.FormatSql,
                env.IsDevelopment().ToString().ToString()
            );
            cfg.AddIdentityMappingsForPostgres();
            services.AddHibernate(configFile);
        }

        private void ConfigureHibernate(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            // do nothing know
        }

    }

}
