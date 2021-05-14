using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        [HttpGet("{z:int}/{y:int}/{x:int}")]
        public async Task<ActionResult> GetAll(int z, int y, int x) {
            try {
                var sql = BuildVectorSql(z, y, x);
                var result = await GetMvtResultAsync(sql);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex.Message);
                return StatusCode(500);
            }
        }

        private async Task<ActionResult> GetMvtResultAsync(string sql) {
            if (string.IsNullOrEmpty(sql)) {
                return NotFound();
            }
            var buffer = await GetMvtBufferAsync(sql);
            if (buffer == null || buffer.Length == 0) {
                return NotFound();
            }
            return File(buffer, "application/vnd.mapbox-vector-tile");
        }

        private string BuildVectorSql(int z, int y, int x) {
            var sqls = new List<string>();
            foreach (var pair in options.Layers) {
                var sql = BuildVectorSqlForLayer(pair.Key, z, y, x);
                if (!string.IsNullOrEmpty(sql)) {
                    sqls.Add(sql);
                }
            }
            if (sqls.Count == 0) {
                return string.Empty;
            }
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("select ((");
            for (var i = 0; i < sqls.Count; i++) {
                sqlBuilder.Append(sqls[i]);
                if (i < sqls.Count - 1) {
                    sqlBuilder.AppendLine(") || (");
                }
            }
            sqlBuilder.AppendLine("))");
            return sqlBuilder.ToString();
        }

        private string BuildVectorSqlForLayer(string layerName, int z, int y, int x) {
            var layer = options.Layers[layerName];
            if (z < layer.Minzoom || z > layer.Maxzoom) {
                return string.Empty;
            }
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("with mvt_geom as (");
            sqlBuilder.AppendLine("  select");
            sqlBuilder.AppendLine("    ST_AsMVTGeom(");
            if (layer.Srid == 3857) {
                sqlBuilder.AppendLine($"      {layer.GeometryColumn},");
            }
            else {
                sqlBuilder.AppendLine($"      ST_Transform({layer.GeometryColumn}, 3857),");
            }
            sqlBuilder.AppendLine($"      ST_TileEnvelope({z}, {x}, {y}),");
            sqlBuilder.AppendLine("      extent => 4096, buffer => 64");
            sqlBuilder.AppendLine($"    ) as {layer.GeometryColumn},");
            sqlBuilder.AppendLine($"    {layer.IdColumn}, {layer.AttributeColumns}");
            sqlBuilder.AppendLine($"  from {layer.Schema}.{layer.TableName}");
            sqlBuilder.AppendLine($"  where {layer.GeometryColumn} && ST_TileEnvelope({z}, {x}, {y}, margin => (64.0 / 4096))");
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine($"select ST_AsMVT(mvt_geom, '{layerName}', 4096, '{layer.GeometryColumn}', '{layer.IdColumn}')");
            sqlBuilder.AppendLine("from mvt_geom");
            return sqlBuilder.ToString();
        }

        private async Task<byte[]> GetMvtBufferAsync(string sql) {
            logger.LogInformation(sql);
            await using var conn = new NpgsqlConnection(options.ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            var result = await cmd.ExecuteScalarAsync();
            await conn.CloseAsync();
            return result as byte[];
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
        public int Minzoom { get; set; } = 1;
        public int Maxzoom { get; set; } = 19;
    }

}
