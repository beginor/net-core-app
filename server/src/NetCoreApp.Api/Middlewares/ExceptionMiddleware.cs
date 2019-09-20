using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class ExceptionMiddleware {

        private readonly RequestDelegate next;
        private readonly IHostingEnvironment env;

        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        public ExceptionMiddleware(
            RequestDelegate next,
            IHostingEnvironment env
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                await this.next.Invoke(context);
            }
            catch (Exception ex) {
                var message = $"Unhandled Exception with {context.Request.Method} {context.Request.Path} .";
                logger.Error(message, ex);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(
                    env.IsDevelopment() ? ex.ToString() : message,
                    Encoding.UTF8
                );
            }
        }
    }

}
