using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class AuditMiddleware {

        private readonly RequestDelegate next;
        private IActionDescriptorCollectionProvider provider;
        private IActionSelector selector;
        private IServiceProvider serviceProvider;
        private ILogger<AuditMiddleware> logger;

        public AuditMiddleware(
            RequestDelegate next,
            IActionDescriptorCollectionProvider provider,
            IActionSelector selector,
            IServiceProvider serviceProvider,
            ILogger<AuditMiddleware> logger
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private ActionDescriptor GetMatchingAction(string path, string httpMethod) {
            var actionDescriptors = provider.ActionDescriptors.Items;
            // match by route template
            var matchingDescriptors = new List<ActionDescriptor>();
            foreach (var actionDescriptor in actionDescriptors) {
                var matchesRouteTemplate = MatchesTemplate(
                    actionDescriptor.AttributeRouteInfo.Template,
                    path
                );
                if (matchesRouteTemplate) {
                    matchingDescriptors.Add(actionDescriptor);
                }
            }
            // match action by using the IActionSelector
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            httpContext.Request.Method = httpMethod;
            var routeContext = new RouteContext(httpContext);
            return selector.SelectBestCandidate(
                routeContext,
                matchingDescriptors.AsReadOnly()
            );
        }


        private bool MatchesTemplate(string routeTemplate, string requestPath) {
            var template = TemplateParser.Parse(routeTemplate);
            var matcher = new TemplateMatcher(
                template,
                GetDefaults(template)
            );
            var values = new RouteValueDictionary();
            return matcher.TryMatch(requestPath, values);
        }

        // This method extracts the default argument values from the template.
        // From https://blog.markvincze.com/matching-route-templates-manually-in-asp-net-core/
        private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate) {
            var result = new RouteValueDictionary();
            foreach (var parameter in parsedTemplate.Parameters) {
                if (parameter.DefaultValue != null) {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }
            return result;
        }

        public Task InvokeAsync(HttpContext context) {
            var auditLog = new AppAuditLogModel {
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                UserName = GetUserName(context),
                StartAt = DateTime.Now
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp)) {
                ip = realIp.ToString();
            }
            auditLog.Ip = ip;
            context.Response.OnStarting(state => {
                var ctx = (HttpContext)state;
                stopwatch.Stop();
                auditLog.Duration = stopwatch.ElapsedMilliseconds;
                ctx.Response.Headers.Add(
                    "X-Response-Time-Milliseconds",
                    auditLog.Duration.ToString()
                );
                auditLog.ResponseCode = context.Response.StatusCode;
                if (GetMatchingAction(auditLog.RequestPath, auditLog.RequestMethod) is ControllerActionDescriptor action) {
                    auditLog.ControllerName = action.ControllerTypeInfo.Name;
                    auditLog.ActionName = action.MethodInfo.Name;
                }
                Task.Run(() => {
                    // using var scope = serviceProvider.CreateScope();
                    var repo = context.RequestServices.GetService<IAppAuditLogRepository>();
                    return repo.SaveAsync(auditLog);
                });
                return Task.CompletedTask;
            }, context);
            return next.Invoke(context);
        }

        private string GetUserName(HttpContext context) {
            var username = "anonymous";
            var request = context.Request;
            string authorization = request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorization)) {
                return username;
            }
            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) {
                return username;
            }
            var token = authorization.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token)) {
                return username;
            }
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token)) {
                var jst = handler.ReadJwtToken(token);
                var claim = jst.Claims.FirstOrDefault(
                    c => c.Type == "unique_name" || c.Type == ClaimTypes.Name
                );
                if (claim != null) {
                    username = claim.Value;
                }
            }
            return username;
        }
    }

}
