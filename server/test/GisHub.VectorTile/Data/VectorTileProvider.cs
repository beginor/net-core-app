using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GisHub.VectorTile.Data {

    public class VectorTileProvider {

        private Dictionary<string, string> connectionStrings;
        private Dictionary<string, VectorTileSource> vectorTileSources;

        private ILogger<VectorTileProvider> logger;

        public VectorTileProvider(
            ILogger<VectorTileProvider> logger,
            IOptionsMonitor<Dictionary<string, string>> connectionStringsMonitor,
            IOptionsMonitor<Dictionary<string, VectorTileSource>> vectorTileSourcesMonitor
        ) {
            this.logger = logger;
            connectionStrings = connectionStringsMonitor.CurrentValue;
            vectorTileSources = vectorTileSourcesMonitor.CurrentValue;
            MergeConnectionStrings();
            connectionStringsMonitor.OnChange(value => {
                connectionStrings = value;
                MergeConnectionStrings();
            });
            vectorTileSourcesMonitor.OnChange(value => {
                vectorTileSources = value;
                MergeConnectionStrings();
            });
        }

        private void MergeConnectionStrings() {
            if (connectionStrings == null) {
                return;
            }
            if (vectorTileSources == null) {
                return;
            }
            foreach (var source in vectorTileSources.Values) {
                if (connectionStrings.ContainsKey(source.ConnectionString)) {
                    source.ConnectionString = connectionStrings[source.ConnectionString];
                }
            }
        }

        public async Task<byte[]> GetTileContentAsync(string source, int z, int y, int x) {
            if (!vectorTileSources.ContainsKey(source)) {
                return null;
            }
            var vectorTileSource = vectorTileSources[source];
            var sql = BuildSqlForVectorSource(vectorTileSource, z, y, x);
            if (string.IsNullOrEmpty(sql)) {
                return null;
            }
            var result = await GetMvtBufferAsync(vectorTileSource, sql);
            return result;
        }

        private string BuildSqlForVectorSource(VectorTileSource source, int z, int y, int x) {
            var sqls = new List<string>();
            foreach (var layer in source.Layers) {
                var sql = BuildSqlForVectorTileLayer(layer, z, y, x);
                if (!string.IsNullOrEmpty(sql)) {
                    sqls.Add(sql);
                }
            }
            if (sqls.Count == 0) {
                return string.Empty;
            }
            if (sqls.Count == 1) {
                return sqls[0];
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

        private string BuildSqlForVectorTileLayer(VectorTileLayer layer, int z, int y, int x) {
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
            sqlBuilder.AppendLine($"select ST_AsMVT(mvt_geom, '{layer.Name}', 4096, '{layer.GeometryColumn}', '{layer.IdColumn}')");
            sqlBuilder.AppendLine("from mvt_geom");
            return sqlBuilder.ToString();
        }

        private async Task<byte[]> GetMvtBufferAsync(VectorTileSource source, string sql) {
            logger.LogInformation(sql);
            await using var conn = new NpgsqlConnection(source.ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            var result = await cmd.ExecuteScalarAsync();
            await conn.CloseAsync();
            return result as byte[];
        }

    }

}
