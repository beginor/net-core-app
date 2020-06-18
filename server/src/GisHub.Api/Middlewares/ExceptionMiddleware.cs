using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Api.Middlewares {

    public class ExceptionMiddleware {

        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment env;

        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<ExceptionMiddleware> logger
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task InvokeAsync(HttpContext context) {
            try {
                return next(context);
            }
            catch (Exception ex) {
                var message = $"Unhandled Exception with {context.Request.Method} {context.Request.Path} .";
                logger.LogError(ex, message);
                context.Response.StatusCode = 500;
                return context.Response.WriteAsync(
                    env.IsDevelopment() ? ex.ToString() : message,
                    Encoding.UTF8
                );
            }
        }
    }

}
