using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class RefererFilteringMiddleware {

        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<RefererFilteringMiddleware> logger;
        private readonly RefererFilteringOptions options;

        public RefererFilteringMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<RefererFilteringMiddleware> logger,
            RefererFilteringOptions options
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task InvokeAsync(HttpContext context) {
            var origions = this.options.Origions;
            if (origions == null || origions.IndexOf("*") > -1) {
                return next(context);
            }
            var referer = context.Request.GetTypedHeaders().Referer;
            if (referer == null) {
                return next(context);
            }
            var refererUrl = referer.ToString();
            var isValidRefer = origions.Any(origion => refererUrl.StartsWith(origion));
            if (!isValidRefer) {
                var message = $"Referer from {refererUrl} is not allowed.";
                logger.LogError(message);
                context.Response.StatusCode = 400; // BadRequest
                return context.Response.WriteAsync(message, Encoding.UTF8);
            }
            return next(context);
        }

    }

    public class RefererFilteringOptions {

        public IList<string> Origions { get; set; }
    }

    public static class RefererFilteringExtensions {

        public static IServiceCollection AddRefererFiltering(
            this IServiceCollection services,
            Action<RefererFilteringOptions> action
        ) {
            var options = new RefererFilteringOptions();
            action(options);
            if (options.Origions == null) {
                options.Origions = new [] { "*" };
            }
            return services.AddSingleton(options);
        }

        public static IApplicationBuilder UseRefererFiltering(
            this IApplicationBuilder app
        ) {
            return app.UseMiddleware<RefererFilteringMiddleware>();
        }
    }

}
