using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using SysEnvironment = System.Environment;
using NHibernate.NetCore;
using NHibernate.Cfg;

namespace Beginor.NetCoreApp.Api {

    public class Startup {

        private readonly IConfiguration config;
        private readonly IHostingEnvironment env;

        private readonly log4net.ILog log = log4net.LogManager.GetLogger(
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
        public void ConfigureServices(IServiceCollection services) {
            ConfigureMvcServices(services);
            ConfigureSwaggerServices(services);
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            log.Debug("Startup configure app.");
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            var pathbase = SysEnvironment.GetEnvironmentVariable(
                "ASPNETCORE_PATHBASE"
            );
            ConfigurePathBase(app, pathbase);
            ConfigureMvc(app);
            ConfigureSwagger(app, pathbase);
            log.Debug("Configure app completed.");
        }

        #region "NHibernate"
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
            services.AddHibernate(configFile);
        }

        private void ConfigureHibernate(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {

        }
        #endregion

        #region "Mvc"
        private void ConfigureMvcServices(IServiceCollection services) {
            services.AddMvc()
                .ConfigureApplicationPartManager(manager => {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(
                        new AssemblyPart(typeof(Startup).Assembly)
                    );
                })
                .AddControllersAsServices()
                .ConfigureApiBehaviorOptions(opts => {
                    opts.SuppressConsumesConstraintForFormFileParameters = true;
                    opts.SuppressInferBindingSourcesForParameters = true;
                    opts.SuppressModelStateInvalidFilter = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private void ConfigureMvc(IApplicationBuilder app) {
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
        #endregion

        #region "Swagger"
        private void ConfigureSwaggerServices(IServiceCollection services) {
            log.Debug("Start add swagger related services...");
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new Info {
                    Title = "NetCoreApp API Help",
                    Version = "1.0.0"
                });
                var dir = new DirectoryInfo(
                    AppDomain.CurrentDomain.BaseDirectory
                );
                var xmlFiles = dir.EnumerateFiles(
                    "*.xml",
                    SearchOption.AllDirectories
                ).Select(f => f.FullName);
                foreach (var xmlFile in xmlFiles) {
                    log.Debug(
                        $"include xml comments {xmlFile} to swagger"
                    );
                    opt.IncludeXmlComments(xmlFile);
                }
            });
            log.Debug("Add swagger related service completed.");
        }
        private void ConfigureSwagger(
            IApplicationBuilder app,
            string pathbase
        ) {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                // c.RoutePrefix = pathbase;
                c.SwaggerEndpoint(
                    pathbase + "/swagger/v1/swagger.json",
                    "NetCoreApp API V1"
                );
            });
        }
        #endregion

        #region "Pathbase"
        private void ConfigurePathBase(
            IApplicationBuilder app,
            string pathbase
        ) {
            if (string.IsNullOrEmpty(pathbase)) {
                return;
            }
            app.UsePathBase(new PathString(pathbase));
            var message = "Hosting pathbase: " + pathbase;
            log.Debug(message);
        }
        #endregion
    }
}
