using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var ebusOptions = config.GetSection("ebus").Get<EBusOptions>();
            services.AddSingleton(ebusOptions);
            services.AddSingleton<YztService>();
            services.AddCors(cors => {
                cors.AddDefaultPolicy(b => {
                    b.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
                });
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UsePathBase("/gmap");
            app.UseStaticFiles();
            app.Map("/ebus", subApp => {
                subApp.UseMiddleware<EBusMiddleware>();
            });
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
