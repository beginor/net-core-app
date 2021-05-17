using System;
using System.Threading.Tasks;
using GisHub.VectorTile.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GisHub.VectorTile.Api {

    [Route("api/vectortiles")]
    public class VectorTileController : Controller {

        private VectorTileProvider provider;
        private ILogger<VectorTileController> logger;

        public VectorTileController(ILogger<VectorTileController> logger, VectorTileProvider provider) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                provider = null;
            }
            base.Dispose(disposing);
        }

        [HttpGet("{source}/{z:int}/{y:int}/{x:int}")]
        public async Task<ActionResult> GetTile(string source, int z, int y, int x) {
            try {
                var buffer = await provider.GetTileContentAsync(source, z, y, x);
                if (buffer == null || buffer.Length == 0) {
                    return NotFound();
                }
                return File(buffer, "application/vnd.mapbox-vector-tile");
            }
            catch (Exception ex) {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

    }

}
