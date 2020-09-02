using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gmap.Api {

    [Route("api/tms")]
    public class TmsController : Controller {

        private string tms = "http://127.0.0.1:9080/geoserver/gwc/service/tms/1.0.0";

        public TmsController() {
        }

        [HttpGet("{tileName}/{z}/{x}/{y}")]
        public async Task<ActionResult> GetTile(string tileName, int z, int x, int y) {
            var iy = (1 << z) - 1 - y;
            var url = tms + $"/{tileName}/{z}/{x}/{iy}.pbf";
            var req = WebRequest.CreateHttp(url);
            req.Method = Request.Method;
            var res = await req.GetResponseAsync() as HttpWebResponse;
            if (res.StatusCode == HttpStatusCode.OK) {
                var contentType = res.ContentType;
                using var ms = new MemoryStream();
                await res.GetResponseStream().CopyToAsync(ms);
                return File(ms.GetBuffer(), contentType);
            }
            return NotFound();
        }

    }

}
