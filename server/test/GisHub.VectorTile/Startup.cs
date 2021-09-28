using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using GisHub.VectorTile.Data;
using SysEnvironment = System.Environment;

namespace GisHub.VectorTile {

    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<Dictionary<string, string>>(
                Configuration.GetSection("connectionStrings")
            );
            services.Configure<Dictionary<string, VectorTileSource>>(
                Configuration.GetSection("vectors")
            );
            services.AddSingleton<VectorTileProvider>();
            var cacheOptions = Configuration.GetSection("cache").Get<CacheOptions>();
            services.AddSingleton(cacheOptions);
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VectorTile", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VectorTile v1"));
            }

            var pathbase = GetAppPathbase();
            if (!string.IsNullOrEmpty(pathbase)) {
                app.UsePathBase(new PathString(pathbase));
                var message = "Hosting pathbase: " + pathbase;
                Console.WriteLine(message);
            }

            app.UseStaticFiles();
            app.UseCors(cors => {
                cors.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .DisallowCredentials();
            });
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        private string GetAppPathbase() {
            return SysEnvironment.GetEnvironmentVariable(
                "ASPNETCORE_PATHBASE"
            );
        }
    }
}
