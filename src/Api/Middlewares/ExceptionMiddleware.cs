using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beginor.NetCoreApp.Api.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    IWebHostEnvironment env,
    ILogger<ExceptionMiddleware> logger
) {

    public async Task InvokeAsync(HttpContext context) {
        try {
            await next(context);
        }
        catch (Exception ex) {
            var message = $"Unhandled Exception with {context.Request.Method} {context.Request.Path} .";
            logger.LogError(ex, message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(
                env.IsDevelopment() ? ex.ToString() : message,
                Encoding.UTF8
            );
        }
    }
}
