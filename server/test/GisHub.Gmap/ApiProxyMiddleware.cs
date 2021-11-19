using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Gmap.Utils;

namespace Gmap {

    public class ApiProxyMiddleware {

        private readonly RequestDelegate next;
        private readonly ILogger<ApiProxyMiddleware> logger;
        private ApiProxyOptions options;
        private HttpClient http;

        public ApiProxyMiddleware(RequestDelegate next, ILogger<ApiProxyMiddleware> logger, IOptionsMonitor<ApiProxyOptions> monitor) {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }
            options = monitor.CurrentValue;
            CreateProxyClient();
            monitor.OnChange(val => {
                options = val;
                CreateProxyClient();
            });
        }

        private void CreateProxyClient() {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (s, cert, chain, sslErr) => true;
            handler.AutomaticDecompression = DecompressionMethods.All;
            handler.AllowAutoRedirect = false;
            http = new HttpClient(handler);
            http.BaseAddress = new Uri(options.GatewayUrl);
        }

        public async Task InvokeAsync(HttpContext context) {
            var request = context.Request;
            var response = context.Response;
            try {
                // request.EnableBuffering();
                var reqMethod = request.Method;
                var queryString = request.QueryString;
                var servicePath = options.GatewayUrl;
                servicePath += queryString.Value;
                logger.LogInformation($"{reqMethod}: {servicePath}");
                var serviceId = GetServiceId(request);
                if (string.IsNullOrEmpty(serviceId)) {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("Can not get serviceId from request!");
                }
                else if (!options.Services.ContainsKey(serviceId)) {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await response.WriteAsync("Unkonwn serviceId!");
                }
                else {
                    var proxyRequest = new HttpRequestMessage();
                    proxyRequest.Method = new HttpMethod(request.Method);
                    proxyRequest.RequestUri = new Uri(servicePath);
                    // add paas headers;
                    var paasHeaders = ProxyUtil.ComputeSignatureHeaders(options.PaasId, options.PaasToken, serviceId);
                    foreach (var pair in paasHeaders) {
                        proxyRequest.Headers.Add(pair.Key, pair.Value);
                    }
                    proxyRequest.Headers.UserAgent.ParseAdd(request.Headers.UserAgent);
                    proxyRequest.Headers.Accept.ParseAdd(request.Headers.Accept);
                    proxyRequest.Headers.AcceptEncoding.ParseAdd(request.Headers.AcceptEncoding);
                    if (proxyRequest.Method == HttpMethod.Post) {
                        var memoryStream = new MemoryStream();
                        await request.BodyReader.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var content = new StreamContent(memoryStream);
                        content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.Headers.ContentType);
                        proxyRequest.Content = content;
                    }
                    //
                    var proxyResponse = await http.SendAsync(proxyRequest);
                    response.StatusCode = (int)proxyResponse.StatusCode;
                    foreach (var header in proxyResponse.Headers) {
                        response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
                    }
                    response.Headers.Remove("Transfer-Encoding");
                    if (proxyResponse.Content != null) {
                        foreach (var header in proxyResponse.Content.Headers) {
                            response.Headers.Add(header.Key, header.Value.ToArray());
                        }
                        await proxyResponse.Content.CopyToAsync(response.Body);
                    }
                }
            }
            catch (HttpRequestException ex) {
                response.StatusCode = (int)HttpStatusCode.BadGateway;
                var error = $"Proxy Server returns: {ex.HResult}, {ex.Message}, {ex.Source}";
                await response.WriteAsync(error);
                logger.LogError(ex, "ApiProxy returns error.");
            }
            catch (Exception ex) {
                response.StatusCode = 500;
                await response.WriteAsync(ex.ToString());
                logger.LogError(ex, "Network error.");
            }
            finally {
                await response.CompleteAsync();
            }
        }

        private string GetServiceId(HttpRequest request) {
            if (request.Path.HasValue) {
                var path = request.Path.ToString().Substring(1);
                var idx = path.IndexOf("/");
                if (idx > 0) {
                    return path.Substring(0, idx);
                }
                return path;
            }
            return string.Empty;
        }
    }
}
