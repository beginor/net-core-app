using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Beginor.NetCoreApp.Api {

    partial class Startup {

        private void ConfigureSwaggerServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            logger.Debug("Start add swagger related services...");
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new OpenApiInfo {
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
                    logger.Debug(
                        $"include xml comments {xmlFile} to swagger"
                    );
                    opt.IncludeXmlComments(xmlFile);
                }
            });
            logger.Debug("Add swagger related service completed.");
        }

        private void ConfigureSwagger(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseSwagger().UseSwaggerUI(options => {
                // options.RoutePrefix = pathbase;
                options.SwaggerEndpoint(
                    GetAppPathbase() + "/swagger/v1/swagger.json",
                    "NetCoreApp API v1.0.0"
                );
                options.DocumentTitle = "NetCoreApp API v1.0.0";
            });
        }

    }

}
