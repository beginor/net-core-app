using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Beginor.NetCoreApp.Common;

namespace Beginor.GisHub.Api.Middlewares {

    public class SpaFailbackMiddleware {

        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<SpaFailbackMiddleware> logger;
        private SpaFailbackOptions options;

        public SpaFailbackMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<SpaFailbackMiddleware> logger,
            IOptionsMonitor<SpaFailbackOptions> monitor
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = monitor.CurrentValue ?? throw new ArgumentNullException(nameof(monitor));
            monitor.OnChange(newVal => {
                this.options = newVal;
            });
        }

        public Task InvokeAsync(HttpContext context) {
            var request = context.Request;
            var reqPath = request.Path.ToString();
            if (!request.IsAjaxRequest() && !string.IsNullOrEmpty(reqPath)) {
                var filePath = Path.Combine(env.WebRootPath, reqPath.Substring(1));
                if (!File.Exists(filePath) && !Directory.Exists(filePath)) {
                    var failback = options.Failbacks.FirstOrDefault(
                        f => f.PathBaseRegex.IsMatch(reqPath.ToLowerInvariant())
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
        private Regex pathBaseRegex;
        public Regex PathBaseRegex {
            get {
                if (string.IsNullOrEmpty(PathBase)) {
                    return null;
                }
                if (pathBaseRegex == null) {
                    pathBaseRegex = new Regex(PathBase);
                }
                return pathBaseRegex;
            }
        }
        public string PathBase { get; set; }
        public string Failback { get; set; }
    }

    public static class SpaFailbackExtensions {

        public static IApplicationBuilder UseSpaFailback(
            this IApplicationBuilder app
        ) {
            return app.UseMiddleware<SpaFailbackMiddleware>();
        }
    }

}
