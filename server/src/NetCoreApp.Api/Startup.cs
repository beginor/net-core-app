using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using SysEnvironment = System.Environment;

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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            AddSwaggerServices(services);
        }
        
        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env
        ) {
            log.Debug("Startup configure app.");
            var pathbase = SysEnvironment.GetEnvironmentVariable(
                "ASPNETCORE_PATHBASE"
            );
            ConfigurePathBase(app, pathbase);
            
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            ConfigSwagger(app, pathbase);
            log.Debug("Configure app completed.");
        }

        #region "Swagger"
        private void AddSwaggerServices(IServiceCollection services) {
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
        
        private void ConfigSwagger(
            IApplicationBuilder app,
            string pathbase
        ) {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                // c.RoutePrefix = pathBase;
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
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
