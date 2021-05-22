using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureSwaggerServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            logger.Debug("Start add swagger related services...");
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "GisHub API Help",
                    Version = "1.0.0"
                });
                var securityScheme = new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Description = "JWT Authorization Header",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    [securityScheme] = new string[0]
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
                options.UseRequestInterceptor("function (req) { if (req.url.endsWith('/api/account') && req.method === 'POST') { var param = JSON.parse(req.body); param.userName = btoa(param.userName); param.password = btoa(param.password); req.body = JSON.stringify(param); } return req; }");
                options.SwaggerEndpoint(
                    GetAppPathbase() + "/swagger/v1/swagger.json",
                    "GisHub API v1.0.0"
                );
                options.DocumentTitle = "GisHub API v1.0.0";
            });
        }

    }

}
