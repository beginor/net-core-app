using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SysEnvironment = System.Environment;

namespace Beginor.NetCoreApp.Entry;

public partial class Startup {

    private readonly IConfiguration config;
    private readonly IWebHostEnvironment env;

    private readonly log4net.ILog logger = log4net.LogManager.GetLogger(
        MethodBase.GetCurrentMethod()!.DeclaringType!
    );

    public Startup(IConfiguration config, IWebHostEnvironment env) {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.env = env ?? throw new ArgumentNullException(nameof(env));
    }

    // This method gets called by the runtime. Use this method to add
    // services to the container.
    public void ConfigureServices(IServiceCollection services) {
        logger.Info("Start configure services ...");
        // app related.
        ConfigureHibernateServices(services, env);
        ConfigureAutoMapperServices(services, env);
        ConfigureAppServices(services, env);
        ConfigureIdentityServices(services, env);
        // cors
        ConfigureCorsServices(services, env);
        // web related.
        ConfigurePathBaseServices(services, env);
        // forwarded headers
        ConfigureForwardedHeadersServices(services, env);
        ConfigureCustomHeaderServices(services, env);
        ConfigureStaticFilesServices(services, env);
        ConfigureSwaggerServices(services, env);
        // routing and mvc
        ConfigureMiddlewareServices(services, env);
        ConfigureRoutingServices(services, env);
        ConfigureAuthenticationServices(services, env);
        ConfigureMvcServices(services, env);
        // wechat
        // ConfigureWeChatServices(services, env);
        logger.Info("Configure services completed!");
    }

    // This method gets called by the runtime. Use this method to configure
    // the HTTP request pipeline.
    public void Configure(WebApplication app) {
        logger.Info("Start configure app.");
        // app related.
        ConfigureHibernate(app, env);
        ConfigureAutoMapper(app, env);
        ConfigureApp(app, env);
        ConfigureIdentity(app, env);
        // forwarded headers
        ConfigureForwardedHeaders(app, env);
        // cors, auth;
        ConfigureCors(app, env);
        // web related.
        ConfigurePathBase(app, env);
        ConfigureCustomHeader(app, env);
        ConfigureStaticFiles(app, env);
        ConfigureSwagger(app, env);
        // routing and mvc
        ConfigureMiddleware(app, env);
        ConfigureRouting(app, env);
        ConfigureAuthentication(app, env);
        ConfigureMvc(app, env);
        // wechat
        // ConfigureWeChat(app, env);
        logger.Info("Configure app completed.");
    }

    private string? GetAppPathbase() {
        return SysEnvironment.GetEnvironmentVariable(
            "ASPNETCORE_PATHBASE"
        );
    }

}
