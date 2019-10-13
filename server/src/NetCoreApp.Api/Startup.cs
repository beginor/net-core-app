using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SysEnvironment = System.Environment;

namespace Beginor.NetCoreApp.Api {

    public partial class Startup {

        private readonly IConfiguration config;
        private readonly IWebHostEnvironment env;

        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType
        );

        public Startup(IConfiguration config, IWebHostEnvironment env) {
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
            // app related.
            ConfigureHibernateServices(services, env);
            ConfigureAutoMapperServices(services, env);
            ConfigureAppServices(services, env);
            ConfigureIdentityServices(services, env);
            // web related.
            ConfigureMiddlewareServices(services, env);
            ConfigurePathBaseServices(services, env);
            ConfigureStaticFilesServices(services, env);
            ConfigureSwaggerServices(services, env);
            // routing , cookie policy, cors, auth;
            ConfigureRoutingServices(services, env);
            ConfigureCookiePolicyServices(services, env);
            ConfigureCorsServices(services, env);
            ConfigureAuthenticationServices(services, env);
            // mvc is last;
            ConfigureMvcServices(services, env);
            logger.Debug("Configure services completed!");
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app
        ) {
            logger.Debug("Start configure app.");
            // app related.
            ConfigureHibernate(app, env);
            ConfigureAutoMapper(app, env);
            ConfigureApp(app, env);
            ConfigureIdentity(app, env);
            // web related.
            ConfigureMiddleware(app, env);
            ConfigurePathBase(app, env);
            ConfigureStaticFiles(app, env);
            ConfigureSwagger(app, env);
            // routing , cookie policy, cors, auth;
            ConfigureRouting(app, env);
            ConfigureCookiePolicy(app, env);
            ConfigureCors(app, env);
            ConfigureAuthentication(app, env);
            // mvc is last;
            ConfigureMvc(app, env);
            logger.Debug("Configure app completed.");
        }

        private string GetAppPathbase() {
            return SysEnvironment.GetEnvironmentVariable(
                "ASPNETCORE_PATHBASE"
            );
        }

    }
}
