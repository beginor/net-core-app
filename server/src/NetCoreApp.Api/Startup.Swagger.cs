using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureSwaggerServices(
            IServiceCollection services,
            IHostingEnvironment env
        ) {
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
            IHostingEnvironment env
        ) {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                // c.RoutePrefix = pathbase;

                c.SwaggerEndpoint(
                    GetAppPathbase() + "/swagger/v1/swagger.json",
                    "NetCoreApp API V1"
                );
            });
        }

    }

}
