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

    public class CustomHeaderMiddleware {
        private readonly RequestDelegate next;
        private readonly CustomHeaderOptions options;

        public CustomHeaderMiddleware(
            RequestDelegate next,
            CustomHeaderOptions options
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task InvokeAsync(HttpContext context) {
            context.Response.OnStarting(state => {
                var ctx = (HttpContext) state;
                var res = ctx.Response;
                foreach (var pair in options.Headers) {
                    if (string.IsNullOrEmpty(pair.Value) && res.Headers.ContainsKey(pair.Key)) {
                        res.Headers.Remove(pair.Key);
                    }
                    else {
                        res.Headers[pair.Key] = pair.Value;
                    }
                }
                return Task.CompletedTask;
            }, context);
            return next(context);
        }

    }

    public class CustomHeaderOptions {
        public IDictionary<string, string> Headers { get; set; }
    }

    public static class CustomHeaderExtensions {

        public static IServiceCollection AddCustomHeader(
            this IServiceCollection services,
            Action<CustomHeaderOptions> action
        ) {
            var options = new CustomHeaderOptions();
            action(options);
            if (options.Headers == null || options.Headers.Count == 0) {
                throw new InvalidOperationException("Custom Headers is empty!");
            }
            services.AddSingleton(options);
            return services;
        }

        public static IApplicationBuilder UseCustomHeader(
            this IApplicationBuilder app
        ) {
            app.UseMiddleware<CustomHeaderMiddleware>();
            return app;
        }

    }

}
