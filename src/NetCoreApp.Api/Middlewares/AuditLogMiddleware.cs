using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;
using Dapper;
using NHibernate;

using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Api.Middlewares;

public class AuditLogMiddleware : Disposable {

    private readonly RequestDelegate next;
    private readonly IActionDescriptorCollectionProvider provider;
    private readonly IActionSelector selector;
    private readonly ILogger<AuditLogMiddleware> logger;
    private ISessionFactory sessionFactory;
    private readonly CancellationTokenSource cts;
    private readonly Channel<AppAuditLog> channel = Channel.CreateUnbounded<AppAuditLog>();

    public AuditLogMiddleware(
        RequestDelegate next,
        IActionDescriptorCollectionProvider provider,
        IActionSelector selector,
        ILogger<AuditLogMiddleware> logger,
        ISessionFactory sessionFactory
    ) {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
        cts = new CancellationTokenSource();
        ThreadPool.QueueUserWorkItem(StartSaveAuditLog, cts.Token, preferLocal: false);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            cts.Cancel();
        }
        base.Dispose(disposing);
    }

    public async Task InvokeAsync(HttpContext context) {
        var auditLog = new AppAuditLog {
            HostName = context.Request.Host.Value,
            RequestPath = context.Request.Path,
            RequestMethod = context.Request.Method,
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
        auditLog.UserName = GetUserName(context);
        auditLog.Duration = stopwatch.ElapsedMilliseconds;
        auditLog.ResponseCode = context.Response.StatusCode;
        var action = GetMatchingAction(auditLog.RequestPath, auditLog.RequestMethod) as ControllerActionDescriptor;
        if (action != null) {
            auditLog.ControllerName = action.ControllerName;
            auditLog.ActionName = action.ActionName;
        }
        if (!channel.Writer.TryWrite(auditLog)) {
            logger.LogError("Can not write audit log to channel, unsabed log is: {0}", auditLog.ToJson());
        }
    }

    private async void StartSaveAuditLog(CancellationToken token) {
        const int batchSize = 128;
        IList<AppAuditLog> logs = new List<AppAuditLog>(batchSize);
        while (!token.IsCancellationRequested) {
            while (await channel.Reader.WaitToReadAsync(token)) {
                while (channel.Reader.TryRead(out var log)) {
                    logs.Add(log);
                    if (logs.Count == batchSize) {
                        await BatchSaveInTransaction(logs);
                        logs.Clear();
                    }
                }
                if (logs.Count > 0) {
                    await BatchSaveInTransaction(logs);
                    logs.Clear();
                }
            }
        }
        if (logs.Count > 0) {
            logger.LogWarning($"Cancellation requested, {logs.Count} unsaved logs:");
            foreach (var log in logs) {
                logger.LogWarning(log.ToJson());
            }
        }
    }

    private async Task BatchSaveInTransaction(IList<AppAuditLog> logs) {
        logger.LogInformation($"Batch save {logs.Count} audit logs to db ...");
        using var session = sessionFactory.OpenStatelessSession();
        var conn = session.Connection;
        var sql = EntityHelper.GenerateInsertSql(typeof(AppAuditLog));
        await using var tx = await conn.BeginTransactionAsync();
        try {
            var rowsAffected = await conn.ExecuteAsync(sql, logs);
            logger.LogInformation($"Save {rowsAffected} app audit logs to db.");
            await tx.CommitAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not save app audit logs.");
            await tx.RollbackAsync();
        }
    }

    private ActionDescriptor? GetMatchingAction(string path, string httpMethod) {
        var actionDescriptors = provider.ActionDescriptors.Items;
        // match by route template
        var matchingDescriptors = new List<ActionDescriptor>();
        foreach (var actionDescriptor in actionDescriptors) {
            var matchesRouteTemplate = MatchesTemplate(
                actionDescriptor.AttributeRouteInfo!.Template!,
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
        string username = "anonymous";
        if (context.User.Identity!.IsAuthenticated) {
            username = context.User.Identity.Name!;
        }
        return username;
    }
}
