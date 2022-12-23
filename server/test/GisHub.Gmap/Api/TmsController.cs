using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Gmap.Api;

/// <summary>
/// Convert tile schema from tms to xyz.
/// </summary>
[Route("api/tms")]
public class TmsController : Controller {

    private string tms = "http://127.0.0.1:6180/geoserver/gwc/service/tms/1.0.0";

    private ILogger<TmsController> logger;

    public TmsController(ILogger<TmsController> logger) {
        this.logger = logger;
    }

    [HttpGet("{tileName}/{z}/{x}/{y}")]
    public async Task<ActionResult> GetTile(string tileName, int z, int x, int y) {
        var iy = (1 << z) - 1 - y;
        var url = tms + $"/{tileName}/{z}/{x}/{iy}.pbf";
        var httpClient = new HttpClient();
        var req = new HttpRequestMessage(new HttpMethod(Request.Method), url);
        try {
            var res = await httpClient.SendAsync(req);
            if (res.IsSuccessStatusCode) {
                var contentType = res.Content.Headers.ContentType!.ToString();
                using var ms = new MemoryStream();
                await res.Content.CopyToAsync(ms);
                return File(ms.GetBuffer(), contentType);
            }
            var reader = new StreamReader(await res.Content.ReadAsStreamAsync());
            var text = await reader.ReadToEndAsync();
            logger.LogError(new Exception("Tile Error"), "Can not load tile.");
            logger.LogError(text);
            return NotFound();
        }
        catch (Exception ex) {
            logger.LogError(new Exception("Tile Error"), "Can not load tile.");
            logger.LogError(ex.ToString());
            return NotFound();
        }
    }

}
