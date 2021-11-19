using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Gmap.Services;

namespace Gmap {

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
            var yztReq = service.CreateHttpRequest(servicePath, reqMethod);
            if (req.Headers.TryGetValue("Accept", out var acceptVal)) {
                yztReq.Accept = acceptVal;
            }
            if (req.Headers.TryGetValue("Accept-Encoding", out var acceptEncodingVal)) {
                yztReq.Headers.Add("Accept-Encoding", acceptEncodingVal);
            }
            if (req.Headers.TryGetValue("Content-Type", out var contentTypeValue)) {
                yztReq.Headers.Add("Content-Type", contentTypeValue);
            }
            if (req.Headers.TryGetValue("User-Agent", out var userAgentVal)) {
                yztReq.Headers.Add("User-Agent", userAgentVal);
            }
            if (reqMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) {
                var stream = await yztReq.GetRequestStreamAsync();
                await req.Body.CopyToAsync(stream);
                await stream.FlushAsync();
            }
            try {
                var yztRes = (HttpWebResponse) await yztReq.GetResponseAsync();
                res.StatusCode = (int)yztRes.StatusCode;
                res.ContentType = yztRes.ContentType;
                res.Headers.Add("Content-Encoding", yztRes.ContentEncoding);
                var responseStream = yztRes.GetResponseStream();
                if (responseStream != null) {
                    if (yztRes.ContentType.Contains("text/xml", StringComparison.OrdinalIgnoreCase)
                        && queryString.Value.Contains("GetCapabilities", StringComparison.OrdinalIgnoreCase)
                    ) {
                        using var streamReader = new StreamReader(responseStream);
                        var streamWriter = new StreamWriter(res.Body);
                        string line;
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
                }
                logger.LogInformation($"Success {reqMethod}: {servicePath}");
            }
            catch (WebException webEx) {
                var errResponse = (HttpWebResponse)webEx.Response;
                logger.LogError($"Failed {reqMethod}: {servicePath}");
                logger.LogError($"Server returns {errResponse.StatusCode}");
                res.StatusCode = (int)errResponse.StatusCode;
                foreach (string key in errResponse.Headers.Keys) {
                    res.Headers.Add(key, errResponse.Headers.Get(key));
                }
                await errResponse.GetResponseStream().CopyToAsync(res.Body, 1024);
            }
            catch (Exception ex) {
                res.StatusCode = 500;
                await res.WriteAsync(ex.ToString());
            }
            finally {
                await res.CompleteAsync();
            }
        }

    }
}
