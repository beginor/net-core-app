using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class AuditMiddleware {

        private readonly RequestDelegate next;
        private IActionDescriptorCollectionProvider provider;
        private IActionSelector selector;
        private IServiceProvider serviceProvider;

        public AuditMiddleware(
            RequestDelegate next,
            IActionDescriptorCollectionProvider provider,
            IActionSelector selector,
            IServiceProvider serviceProvider
        ) {
            this.next = next;
            this.provider = provider;
            this.selector = selector;
            this.serviceProvider = serviceProvider;
        }

        public ActionDescriptor GetMatchingAction(string path, string httpMethod) {
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


        public bool MatchesTemplate(string routeTemplate, string requestPath) {
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

        public async Task InvokeAsync(HttpContext context) {
            var log = new AppAuditLog {
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method
            };
            if (context.User.Identity.IsAuthenticated) {
                log.UserName = context.User.Identity.Name;
            }
            log.StartAt = DateTime.Now;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await next.Invoke(context);
            stopwatch.Stop();
            log.Duration = stopwatch.ElapsedMilliseconds;
            log.ResponseCode = context.Response.StatusCode;
            var action = GetMatchingAction(
                log.RequestPath,
                log.RequestMethod
            ) as ControllerActionDescriptor;
            if (action != null) {
                log.ControllerName = action.ControllerTypeInfo.Name;
                log.ActionName = action.MethodInfo.Name;
            }
            using (var scope = serviceProvider.CreateScope()) {
                var repo = scope.ServiceProvider.GetService<IAppAuditLogRepository>();
                await repo.SaveAsync(log);
            }
        }

    }

}
