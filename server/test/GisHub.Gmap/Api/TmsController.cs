using System;
using System.IO;
using System.Net;
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
        var req = WebRequest.CreateHttp(url);
        req.Method = Request.Method;
        try {
            var res = await req.GetResponseAsync() as HttpWebResponse;
            if (res!.StatusCode == HttpStatusCode.OK) {
                var contentType = res.ContentType;
                using var ms = new MemoryStream();
                await res.GetResponseStream().CopyToAsync(ms);
                return File(ms.GetBuffer(), contentType);
            }
            else {
                var reader = new StreamReader(res.GetResponseStream());
                var text = reader.ReadToEnd();
                logger.LogError(new Exception("Tile Error"), "Can not load tile.");
                logger.LogError(text);
                return NotFound();
            }
        }
        catch (Exception ex) {
            logger.LogError(new Exception("Tile Error"), "Can not load tile.");
            logger.LogError(ex.ToString());
            return NotFound();
        }
    }

}
