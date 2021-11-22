using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Gmap.Utils;

namespace Gmap.Api {

    [Route("api/proxy")]
    public class ProxyController : Controller {

        private readonly ILogger<ProxyController> logger;
        private ApiProxyOptions options;

        public ProxyController(ILogger<ProxyController> logger, IOptionsMonitor<ApiProxyOptions> monitor) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (monitor == null) {
                throw new ArgumentNullException(nameof(monitor));
            }
            options = monitor.CurrentValue;
        }

        [HttpGet("{serviceId}")]
        public Task<ActionResult> InvokeByGet(string serviceId) {
            return Invoke(serviceId);
        }

        [HttpPost("{serviceId}")]
        [EnableRequestBuffering]
        [Consumes("application/xml", "text/xml")]
        public Task<ActionResult> InvokeByPost(string serviceId) {
            if (Request.Body.CanSeek && Request.Body.Position != 0) {
                Request.Body.Seek(0, SeekOrigin.Begin);
            }
            return Invoke(serviceId);
        }

        private async Task<ActionResult> Invoke(string serviceId) {
            var svc = options.FindServiceById(serviceId);
            if (svc == null) {
                return BadRequest($"Unknown serviceId {serviceId}");
            }
            logger.LogInformation($"Request serviceId is: {serviceId}");
            try {
                var proxyRequest = await ProxyUtil.CreateProxyHttpRequestMessage(Request, options.GatewayUrl);
                // add paas headers;
                ProxyUtil.AddSignatureHeaders(proxyRequest.Headers, options.PaasId, options.PaasToken, serviceId, logger);
                using var httpClient = CreateProxyClient(options.GatewayUrl);
                // send request to proxy;
                logger.LogInformation("{0}:{1}", proxyRequest.Method, proxyRequest.RequestUri);
                var proxyResponse = await httpClient.SendAsync(proxyRequest);
                CopyHeaderToResponse(proxyResponse.Headers, Response.Headers, "X-Application-Context", "task_id", "sender_id", "service_id");
                // process response
                var contentStream = new MemoryStream();
                var content = proxyResponse.Content;
                var contentType = content.Headers.ContentType;
                // replace proxy url;
                string op = Request.Query["request"];
                var mediaType = contentType?.MediaType;
                if (ProxyUtil.NeedReplace(op, mediaType)) {
                    var replacement = Request.Scheme + "://" + Request.Host + Request.PathBase + Request.Path;
                    await ProxyUtil.ReplaceContent(await content.ReadAsStreamAsync(), contentStream, replacement);
                }
                else {
                    await content.CopyToAsync(contentStream);
                }
                contentStream.Seek(0, SeekOrigin.Begin);
                return File(contentStream, contentType.ToString());
            }
            catch (HttpRequestException ex) {
                var errorMessage = $"Get {ex.StatusCode} from {options.GatewayUrl}, {ex.Message}";
                logger.LogError(ex, errorMessage);
                return StatusCode((int)HttpStatusCode.BadGateway, errorMessage);
            }
            catch (Exception ex) {
                var errorMessage = $"Can not connect to {options.GatewayUrl}";
                logger.LogError(ex, errorMessage);
                return this.StatusCode((int)HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        private void CopyHeaderToResponse(HttpResponseHeaders proxyHeaders, IHeaderDictionary responseHeaders, params string[] names) {
            foreach (var name in names) {
                if (proxyHeaders.TryGetValues(name, out var values)) {
                    var headerValues = new StringValues(values.ToArray());
                    logger.LogInformation($"{name}: headerValues");
                    responseHeaders.Add(name, headerValues);
                }
            }
        }

        private static HttpClient CreateProxyClient(string baseUrl) {
            var handler = new HttpClientHandler() {
                ServerCertificateCustomValidationCallback = (s, cert, chain, sslErr) => true,
                AutomaticDecompression = DecompressionMethods.All,
                AllowAutoRedirect = false,
            };
            var http = new HttpClient(handler);
            return http;
        }

    }

}
