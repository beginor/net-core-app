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
        private readonly YztService service;

        public EBusMiddleware(
            RequestDelegate next,
            ILogger<EBusMiddleware> logger,
            YztService service
        ) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task InvokeAsync(HttpContext context) {
            var req = context.Request;
            var res = context.Response;
            var reqPath = req.Path.ToString();
            var reqMethod = req.Method;
            var queryString = req.QueryString;
            var servicePath = service.GetGatewayServiceUrl(reqPath);
            if (queryString != null) {
                servicePath += queryString.Value;
            }
            logger.LogInformation($"{reqMethod}: {servicePath}");
            var yztReq = HttpWebRequest.CreateHttp(servicePath);
            yztReq.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
                return true;
            };
            yztReq.Method = reqMethod;
            yztReq.AutomaticDecompression = DecompressionMethods.All;
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
            var paasHeaders = service.ComputeSignatureHeaders();
            foreach (var pair in paasHeaders) {
                yztReq.Headers.Add(pair.Key, pair.Value);
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
                if (yztRes.ContentType.Contains("text/xml", StringComparison.OrdinalIgnoreCase)
                    && queryString != null
                    && queryString.Value.Contains("GetCapabilities", StringComparison.OrdinalIgnoreCase)
                ) {
                    using var streamReader = new StreamReader(yztRes.GetResponseStream());
                    var streamWriter = new StreamWriter(res.Body);
                    var line = string.Empty;
                    var replacement = req.Scheme + "://" + req.Host + req.PathBase;
                    while ((line = await streamReader.ReadLineAsync()) != null) {
                        line = service.ReplaceGatewayUrl(line, replacement);
                        await streamWriter.WriteLineAsync(line);
                    }
                    await streamWriter.FlushAsync();
                }
                else {
                    await yztRes.GetResponseStream().CopyToAsync(res.Body, 1024);
                    // await CopyStreamAsync(yztRes.GetResponseStream(), res.Body);
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
                // await CopyStreamAsync(errResponse.GetResponseStream(), res.Body);
            }
            catch (Exception ex) {
                res.StatusCode = 500;
                await res.WriteAsync(ex.ToString());
            }
            finally {
                await res.CompleteAsync();
            }
        }

        private static async Task CopyStreamAsync(Stream input, Stream output) {
            byte[] buffer = new byte[1024];
            var count = 0;
            while ((count = await input.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                await output.WriteAsync(buffer, 0, count);
            }
            await output.FlushAsync();
        }

    }

}
