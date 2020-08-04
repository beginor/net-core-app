using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class CustomHeaderMiddleware {
        private readonly RequestDelegate next;
        private CustomHeaderOptions options;

        public CustomHeaderMiddleware(
            RequestDelegate next,
            IOptionsMonitor<CustomHeaderOptions> monitor
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.options = monitor.CurrentValue ?? throw new ArgumentNullException(nameof(monitor));
            monitor.OnChange(newValue => options = newValue);
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

        public static IApplicationBuilder UseCustomHeader(
            this IApplicationBuilder app
        ) {
            app.UseMiddleware<CustomHeaderMiddleware>();
            return app;
        }

    }

}
