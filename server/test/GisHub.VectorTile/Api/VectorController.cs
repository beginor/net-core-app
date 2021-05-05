using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dapper;
using Npgsql;

namespace GisHub.VectorTile.Api {

    [Route("api/vector")]
    public class VectorController : Controller {

        private VectorTileOptions options;
        private ILogger<VectorController> logger;

        public VectorController(
            ILogger<VectorController> logger,
            IOptionsSnapshot<VectorTileOptions> snapshot
        ) {
            this.logger = logger;
            this.options = snapshot.Value;
        }

        [HttpGet("{layer}/{z:int}/{y:int}/{x:int}")]
        public async Task<ActionResult> Get(string layer, int z, int y, int x) {
            if (!options.Layers.ContainsKey(layer)) {
                return NotFound();
            }
            try {
                var buffer = await GetMvtAsync(layer, z, y, x);
                return File(buffer, "application/vnd.mapbox-vector-tile");
            }
            catch (Exception ex) {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        private async Task<byte[]> GetMvtAsync(string layer, int z, int y, int x) {
            var vectorLayer = options.Layers[layer];
            var sql = new StringBuilder();
            sql.AppendLine("with mvt_geom as (");
            sql.AppendLine("  select");
            sql.AppendLine("    ST_AsMVTGeom(");
            if (vectorLayer.Srid == 3857) {
                sql.AppendLine($"      {vectorLayer.GeometryColumn},");
            }
            else {
                sql.AppendLine($"      ST_Transform({vectorLayer.GeometryColumn}, 3857),");
            }
            sql.AppendLine($"      ST_TileEnvelope({z}, {x}, {y}),");
            sql.AppendLine("      extent => 4096, buffer => 64");
            sql.AppendLine($"    ) as {vectorLayer.GeometryColumn},");
            sql.AppendLine($"    {vectorLayer.IdColumn}, {vectorLayer.AttributeColumns}");
            sql.AppendLine($"  from {vectorLayer.Schema}.{vectorLayer.TableName}");
            sql.AppendLine($"  where {vectorLayer.GeometryColumn} && ST_TileEnvelope({z}, {x}, {y}, margin => (64.0 / 4096))");
            sql.AppendLine(")");
            sql.AppendLine($"select ST_AsMVT(mvt_geom, '{layer}', 4096, '{vectorLayer.GeometryColumn}', '{vectorLayer.IdColumn}')");
            sql.AppendLine("from mvt_geom;");
            logger.LogInformation(sql.ToString());
            await using var conn = new NpgsqlConnection(options.ConnectionString);
            var buffer = await conn.ExecuteScalarAsync<byte[]>(sql.ToString());
            return buffer;
        }

    }

    public class VectorTileOptions {
        public string ConnectionString { get; set; }
        public Dictionary<string, VectorLayer> Layers { get; set; }
    }

    public class VectorLayer {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string IdColumn { get; set; }
        public string AttributeColumns { get; set; }
        public string GeometryColumn { get; set; }
        public int Srid { get; set; }
    }

}
