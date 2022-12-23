using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Beginor.GisHub.Gmap.Services;

namespace Beginor.GisHub.Gmap;

public class EBusMiddleware {

    private readonly RequestDelegate next;
    private readonly ILogger<EBusMiddleware> logger;
    protected readonly YztService service;

    public EBusMiddleware(
        RequestDelegate next,
        ILogger<EBusMiddleware> logger,
        YztService service
    ) {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public virtual async Task InvokeAsync(HttpContext context) {
        var req = context.Request;
        var res = context.Response;
        var reqPath = req.Path.ToString();
        var reqMethod = req.Method;
        var queryString = req.QueryString;
        var servicePath = service.GetGatewayServiceUrl(reqPath);
        servicePath += queryString.Value;
        logger.LogInformation($"{reqMethod}: {servicePath}");
        var yztReq = service.CreateHttpRequestMessage(new HttpMethod(reqMethod), servicePath);
        if (req.Headers.TryGetValue("Accept", out var acceptVal)) {
            yztReq.Headers.Accept.TryParseAdd(acceptVal);
        }
        if (req.Headers.TryGetValue("Accept-Encoding", out var acceptEncodingVal)) {
            yztReq.Headers.AcceptEncoding.TryParseAdd(acceptEncodingVal);
        }
        if (req.Headers.TryGetValue("User-Agent", out var userAgentVal)) {
            yztReq.Headers.UserAgent.TryParseAdd(userAgentVal);
        }
        if (reqMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) {
            var stream = new MemoryStream();
            await req.Body.CopyToAsync(stream);
            await stream.FlushAsync();
            var content = new StreamContent(stream);
            if (req.Headers.TryGetValue("Content-Type", out var contentTypeValue)) {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentTypeValue);
            }
        }
        
        using var yztRes = await service.SendAsync(yztReq);
        if (yztRes == null) {
            res.StatusCode = (int)HttpStatusCode.BadGateway;
            await res.WriteAsync($"Can not get response from {reqMethod} {servicePath}");
            await res.CompleteAsync();
            return;
        }
        res.StatusCode = (int)yztRes.StatusCode;
        var contentType = yztRes.Content.Headers.ContentType?.ToString() ?? string.Empty;
        res.ContentType = contentType;
        res.Headers.Add("Content-Encoding", yztRes.Content.Headers.ContentEncoding.ToString());
        var responseStream = await yztRes.Content.ReadAsStreamAsync();
        if (yztRes.IsSuccessStatusCode
            && contentType.Contains("text/xml", StringComparison.OrdinalIgnoreCase)
            && queryString.Value!.Contains("GetCapabilities", StringComparison.OrdinalIgnoreCase)
        ) {
            using var streamReader = new StreamReader(responseStream);
            var streamWriter = new StreamWriter(res.Body);
            string? line;
            var replacement = req.Scheme + "://" + req.Host + req.PathBase;
            while ((line = await streamReader.ReadLineAsync()) != null) {
                line = service.ReplaceGatewayUrl(line, replacement);
                await streamWriter.WriteLineAsync(line);
            }
            await streamWriter.FlushAsync();
        }
        else {
            await responseStream.CopyToAsync(res.Body, 1024);
        }
        await res.CompleteAsync();
        logger.LogInformation($"Success {reqMethod}: {servicePath}");
    }

}
