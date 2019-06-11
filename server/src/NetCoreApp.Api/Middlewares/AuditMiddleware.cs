using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class AuditMiddleware {

        private readonly RequestDelegate next;
        IActionDescriptorCollectionProvider provider;
        IActionSelector selector;

        public AuditMiddleware(
            RequestDelegate next,
            IActionDescriptorCollectionProvider provider,
            IActionSelector selector
        ) {
            this.next = next;
            this.provider = provider;
            this.selector = selector;
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
            var action = GetMatchingAction(
                context.Request.Path,
                context.Request.Method
            );
            await next.Invoke(context);
            var responseCode = context.Response.StatusCode;
            System.Console.WriteLine(responseCode);
        }
    }
}
