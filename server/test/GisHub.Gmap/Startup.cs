using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Gmap.Services;

namespace Gmap {

    public class Startup {

        private readonly IConfiguration config;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration config, IWebHostEnvironment env) {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void ConfigureServices(IServiceCollection services) {
            services.Configure<EBusOptions>(config.GetSection("ebus"));
            services.AddSingleton<YztService>();
            services.Configure<ApiProxyOptions>(config.GetSection("apiProxy"));
            var corsPolicy = config.GetSection("cors").Get<CorsPolicy>();
            services.AddCors(cors => {
                cors.AddDefaultPolicy(corsPolicy);
            });
            services.AddControllers();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UsePathBase("/gmap");
            app.UseStaticFiles();
            app.UseCors();
            app.Map("/ebus", subApp => {
                subApp.UseMiddleware<EBusMiddleware>();
            });
            app.Map("/proxy", subApp => {
                subApp.UseMiddleware<ApiProxyMiddleware>();
            });
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
