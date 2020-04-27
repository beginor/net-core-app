using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class SpaFailbackMiddleware {

        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<SpaFailbackMiddleware> logger;
        private readonly SpaFailbackOptions options;

        public SpaFailbackMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<SpaFailbackMiddleware> logger,
            SpaFailbackOptions options
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task InvokeAsync(HttpContext context) {
            var request = context.Request;
            var reqPath = request.Path.ToString();
            if (!string.IsNullOrEmpty(reqPath)) {
                var filePath = Path.Combine(env.WebRootPath, reqPath.Substring(1));
                if (!File.Exists(filePath) && !Directory.Exists(filePath)) {
                    var failback = options.Failbacks.FirstOrDefault(
                        f => reqPath.StartsWith(f.PathBase, StringComparison.OrdinalIgnoreCase)
                    );
                    if (failback != null) {
                        request.Path = failback.Failback;
                    }
                }
            }
            return next(context);
        }
    }

    public class SpaFailbackOptions {
        public IList<SpaFailback> Failbacks { get; set; }
    }

    public class SpaFailback {
        public string PathBase { get; set; }
        public string Failback { get; set; }
    }

    public static class SpaFailbackExtensions {

        public static IServiceCollection AddSpaFailback(
            this IServiceCollection services,
            Action<SpaFailbackOptions> action
        ) {
            var options = new SpaFailbackOptions();
            action(options);
            if (options.Failbacks == null || options.Failbacks.Count == 0) {
                throw new InvalidOperationException("SpaFailbacks is empty!");
            }
            services.AddSingleton(options);
            return services;
        }

        public static IApplicationBuilder UseSpaFailback(
            this IApplicationBuilder app
        ) {
            return app.UseMiddleware<SpaFailbackMiddleware>();
        }
    }

}
