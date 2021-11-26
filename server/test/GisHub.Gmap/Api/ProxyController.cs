using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Gmap.Data;
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

        [HttpGet("{serviceId}/tile/{level:int}/{row:int}/{col:int}")]
        public async Task<ActionResult> GetTile(
            [FromRoute]string serviceId,
            [FromRoute]int level,
            [FromRoute]int row,
            [FromRoute]int col
        ) {
            var svc = options.FindServiceById(serviceId);
            if (svc == null) {
                return BadRequest($"Unknown serviceId {serviceId}");
            }
            logger.LogInformation($"Get tile for service {serviceId}");
            try {
                var z = level;
                var extent = new [] {
                    new [] { MercatorTileUtil.TileX2Lng(col, z), MercatorTileUtil.TileY2Lat(row, z) },
                    new [] { MercatorTileUtil.TileX2Lng(col + 1, z), MercatorTileUtil.TileY2Lat(row + 1, z) }
                };
                // 切片宽度一致， 只需要求切片上下两边的中间点所对应的切片编号即可；
                var halfTileWidth = 360.0 / (1 << z + 1);
                var startX = YztTileUtil.Lng2TileX(extent[0][0] + halfTileWidth, z);
                var startY = YztTileUtil.Lat2TileY(extent[0][1], z);
                var endX = YztTileUtil.Lng2TileX(extent[1][0] - halfTileWidth, z);
                var endY = YztTileUtil.Lat2TileY(extent[1][1], z);
                var tiles = new List<Tile>();
                for (var y = startY; y <= endY; y++) {
                    for (var x = startX; x <= endX; x++) {
                        tiles.Add(new Tile(x, y, z));
                    }
                }
                // 从粤政图服务获取切片内容
                var httpClient = ProxyUtil.CreateHttpClient(svc.GatewayUrl);
                var tileUrlTemplate = $"{svc.GatewayUrl}?{svc.TileTemplate}";
                foreach (var tile in tiles) {
                    var tileUrl = string.Format(tileUrlTemplate, tile.Z, tile.Y, tile.X);
                    var tileRequest = await ProxyUtil.CreateHttpRequestMessage(Request, new Uri(tileUrl));
                    ProxyUtil.AddSignatureHeaders(tileRequest.Headers, svc.PaasId, svc.PaasToken, serviceId, logger);
                    try {
                        var tileResponse = await httpClient.SendAsync(tileRequest);
                        var mediaType = tileResponse.Content.Headers.ContentType.MediaType;
                        if (mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) {
                            var stream = new MemoryStream();
                            await tileResponse.Content.CopyToAsync(stream);
                            tile.Content = stream.GetBuffer();
                            tile.ContentType = mediaType;
                        }
                    }
                    catch (Exception ex) {
                        logger.LogError(ex, $"Get tile from {tileUrl} with ex.");
                    }
                }
                // 切片内容合成一幅图片
                var result = YztTileUtil.CropTiles(tiles, extent);
                if (result == null) {
                    return NotFound();
                }
                return File(result, "image/png");
            }
            catch (Exception ex) {
                var message = $"Can not get tile for {serviceId}/{level}/{row}/{col}";
                logger.LogError(ex, message);
                return StatusCode((int)HttpStatusCode.InternalServerError, message);
            }
        }

        private async Task<ActionResult> Invoke(string serviceId) {
            var svc = options.FindServiceById(serviceId);
            if (svc == null) {
                return BadRequest($"Unknown serviceId {serviceId}");
            }
            logger.LogInformation($"Request serviceId is: {serviceId}");
            try {
                var serviceUrl = svc.GatewayUrl + Request.QueryString;
                var proxyRequest = await ProxyUtil.CreateHttpRequestMessage(Request, new Uri(serviceUrl));
                // add paas headers;
                ProxyUtil.AddSignatureHeaders(proxyRequest.Headers, svc.PaasId, svc.PaasToken, serviceId, logger);
                using var httpClient = ProxyUtil.CreateHttpClient(svc.GatewayUrl);
                // send request to proxy;
                logger.LogInformation("{0}:{1}", proxyRequest.Method, proxyRequest.RequestUri);
                var proxyResponse = await httpClient.SendAsync(proxyRequest);
                ProxyUtil.CopyHeaderToResponse(proxyResponse.Headers, Response.Headers, logger, "X-Application-Context", "task_id", "sender_id", "service_id");
                // process response
                var contentStream = new MemoryStream();
                var content = proxyResponse.Content;
                var contentType = content.Headers.ContentType;
                // replace proxy url;
                string op = Request.Query["request"];
                var mediaType = contentType?.MediaType;
                if (ProxyUtil.NeedReplace(op, mediaType)) {
                    var replacement = Request.Scheme + "://" + Request.Host + Request.PathBase + Request.Path;
                    await ProxyUtil.ReplaceInStream(await content.ReadAsStreamAsync(), contentStream, replacement);
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

    }

}
