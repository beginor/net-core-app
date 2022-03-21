using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beginor.GisHub.Api.Middlewares; 

public class RefererFilteringMiddleware {

    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<RefererFilteringMiddleware> logger;
    private CorsPolicy policy;

    public RefererFilteringMiddleware(
        RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<RefererFilteringMiddleware> logger,
        IOptionsMonitor<CorsPolicy> monitor
    ) {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.env = env ?? throw new ArgumentNullException(nameof(env));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.policy = monitor.CurrentValue ?? throw new ArgumentNullException(nameof(monitor));
        monitor.OnChange(newVal => this.policy = newVal);
    }

    public Task InvokeAsync(HttpContext context) {
        var origions = this.policy.Origins;
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

public static class RefererFilteringExtensions {

    public static IApplicationBuilder UseRefererFiltering(this WebApplication app) {
        return app.UseMiddleware<RefererFilteringMiddleware>();
    }

}