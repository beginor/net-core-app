using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SysEnvironment = System.Environment;

namespace Beginor.NetCoreApp.Api {

    public partial class Startup {

        private readonly IConfiguration config;
        private readonly IHostingEnvironment env;

        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType
        );

        public Startup(IConfiguration config, IHostingEnvironment env) {
            this.config = config ?? throw new ArgumentNullException(
                nameof(config)
            );
            this.env = env ?? throw new ArgumentNullException(
                nameof(env)
            );
        }

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        public void ConfigureServices(
            IServiceCollection services
        ) {
            logger.Debug("Start configure services ...");
            ConfigureHibernateServices(services, env);
            ConfigureIdentityServices(services, env);
            ConfigurePathBaseServices(services, env);
            ConfigureAuthenticationServices(services, env);
            ConfigureCookiePolicyServices(services, env);
            ConfigureSwaggerServices(services, env);
            ConfigureStaticFilesServices(services, env);
            ConfigureCorsServices(services, env);
            ConfigureMvcServices(services, env);
            ConfigureAppServices(services, env);
            logger.Debug("Configure services completed!");
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app
        ) {
            logger.Debug("Start configure app.");
            if (env.IsDevelopment()) {
                // app.UseDeveloperExceptionPage();
            }
            ConfigureHibernate(app, env);
            ConfigureIdentity(app, env);
            ConfigurePathBase(app, env);
            ConfigureAuthentication(app, env);
            ConfigureCookiePolicy(app, env);
            ConfigureSwagger(app, env);
            ConfigureStaticFiles(app, env);
            ConfigureCors(app, env);
            ConfigureMvc(app, env);
            ConfigureApp(app, env);
            logger.Debug("Configure app completed.");
        }

        private string GetAppPathbase() {
            return SysEnvironment.GetEnvironmentVariable(
                "ASPNETCORE_PATHBASE"
            );
        }

    }
}
