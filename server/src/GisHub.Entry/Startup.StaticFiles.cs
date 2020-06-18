using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.Api.Middlewares;
using Microsoft.AspNetCore.StaticFiles;

namespace Beginor.GisHub.Entry {

    partial class Startup {

        private void ConfigureStaticFilesServices(
            IServiceCollection services,
            IWebHostEnvironment env
        ) {
            services.AddSpaFailback(options => {
                var section = config.GetSection("spaFailback");
                section.Bind(options);
            });
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
#if DEBUG
            services.AddDirectoryBrowser();
#endif
        }

        private void ConfigureStaticFiles(
            IApplicationBuilder app,
            IWebHostEnvironment env
        ) {
            app.UseDefaultFiles();
            app.UseGzipStatic();
            app.UseSpaFailback();
            app.UseStaticFiles();
#if DEBUG
            app.UseDirectoryBrowser();
#endif
        }

    }
}
