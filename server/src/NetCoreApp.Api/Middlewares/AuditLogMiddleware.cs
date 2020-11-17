using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
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
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Api.Middlewares {

    public class AuditLogMiddleware : Disposable {

        private readonly RequestDelegate next;
        private IActionDescriptorCollectionProvider provider;
        private IActionSelector selector;
        private ILogger<AuditLogMiddleware> logger;
        private NHibernate.ISession session;
        private IServiceScope scope;
        private ConcurrentQueue<AppAuditLog> logQueue = new ConcurrentQueue<AppAuditLog>();
        private CancellationTokenSource cts;

        public AuditLogMiddleware(
            RequestDelegate next,
            IActionDescriptorCollectionProvider provider,
            IActionSelector selector,
            IServiceProvider serviceProvider,
            ILogger<AuditLogMiddleware> logger
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            if (serviceProvider == null) {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scope = serviceProvider.CreateScope();
            this.session = this.scope.ServiceProvider.GetService<NHibernate.ISession>();
            cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(StartSaveAuditLog, cts.Token, preferLocal: false);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.cts.Cancel();
                this.session.Dispose();
                this.scope.Dispose();
                // this.next = null;
                this.provider = null;
                this.selector = null;
                // this.serviceProvider = null;
                this.logger = null;
            }
            base.Dispose(disposing);
        }

        public async Task InvokeAsync(HttpContext context) {
            var auditLog = new AppAuditLog {
                HostName = context.Request.Host.Value,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                UserName = GetUserName(context),
                StartAt = DateTime.Now,
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (context.Request.HttpContext.Connection.RemoteIpAddress != null) {
                var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp)) {
                    ip = realIp.ToString();
                }
                auditLog.Ip = ip;
            }
            await next.Invoke(context);
            stopwatch.Stop();
            auditLog.Duration = stopwatch.ElapsedMilliseconds;
            auditLog.ResponseCode = context.Response.StatusCode;
            var action = GetMatchingAction(auditLog.RequestPath, auditLog.RequestMethod) as ControllerActionDescriptor;
            if (action != null) {
                auditLog.ControllerName = action.ControllerName;
                auditLog.ActionName = action.ActionName;
            }
            logQueue.Enqueue(auditLog);
        }

        private void StartSaveAuditLog(CancellationToken token) {
            const int batchSize = 128;
            IList<AppAuditLog> logs = new List<AppAuditLog>(batchSize);
            while (!token.IsCancellationRequested) {
                if (logQueue.Count == 0) {
                    logger.LogInformation("Audit log queue is empty, wait for 1 second to check again.");
                    Thread.Sleep(1000);
                    continue;
                }
                while (logQueue.TryDequeue(out var log)) {
                    logs.Add(log);
                    if (logs.Count == batchSize) {
                        BatchSaveInTransaction(logs);
                        logs.Clear();
                    }
                }
                if (logs.Count > 0) {
                    BatchSaveInTransaction(logs);
                }
                logs.Clear();
            }
            if (logs.Count > 0) {
                logger.LogWarning($"Cancellation requested, {logs.Count} unsaved logs:");
                foreach (var log in logs) {
                    logger.LogWarning(log.ToJson());
                }
            }
        }

        private void BatchSaveInTransaction(IList<AppAuditLog> logs) {
            logger.LogInformation($"Audit log queue size is {logQueue.Count} ");
            logger.LogInformation($"Batch save {logs.Count} audit logs to db ...");
            session.SetBatchSize(logs.Count);
            using (var tx = session.BeginTransaction()) {
                try {
                    session.SetBatchSize(logs.Count);
                    foreach (var log in logs) {
                        session.Save(log);
                    }
                    session.Flush();
                    session.Clear();
                    tx.Commit();
                }
                catch (Exception ex) {
                    tx?.Rollback();
                    logger.LogError(ex, "Can not save audit logs with transactions.");
                    foreach (var log in logs) {
                        logger.LogError(log.ToJson());
                    }
                }
            }
        }

        private ActionDescriptor GetMatchingAction(string path, string httpMethod) {
            var actionDescriptors = provider.ActionDescriptors.Items;
            // match by route template
            var matchingDescriptors = new List<ActionDescriptor>();
            foreach (var actionDescriptor in actionDescriptors) {
                var matchesRouteTemplate = MatchesTemplate(
                    actionDescriptor.AttributeRouteInfo!.Template,
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
                    result.Add(parameter.Name!, parameter.DefaultValue);
                }
            }
            return result;
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
