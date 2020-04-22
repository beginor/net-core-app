using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Beginor.NetCoreApp.Api.Middlewares;

namespace Beginor.NetCoreApp.Entry {

    partial class Startup {

        private void ConfigureCorsServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            var corsPolicy = config.GetSection("corsPolicy").Get<CorsPolicy>();
            // Referer
            services.AddCors(corsOptions => {
                corsOptions.AddDefaultPolicy(corsPolicy);
            });
            services.AddRefererFiltering(options => {
                options.Origions = corsPolicy.Origins;
            });
        }

        private void ConfigureCors(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseCors();
            app.UseRefererFiltering();
        }

    }

}
