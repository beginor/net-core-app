using System.Text;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Esri;
using Beginor.GisHub.DataServices.Models;
using Dapper;
using Npgsql;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISFeatureProvider : FeatureProvider {

        public PostGISFeatureProvider(
            IDataServiceFactory factory
        ) : base(factory) { }

        protected override async Task<int> GetSridAsync(DataSourceCacheItem ds) {
            var sql = new StringBuilder();
            sql.AppendLine($" select st_srid({ds.GeometryColumn}) ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            sql.AppendLine($" where {ds.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var srid = await conn.ExecuteScalarAsync<int>(sql.ToString());
            return srid;
        }

        protected override async Task<string> GetGeometryTypeAsync(DataSourceCacheItem ds) {
            var sql = new StringBuilder();
            sql.AppendLine($" select st_geometrytype({ds.GeometryColumn}) ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            sql.AppendLine($" where {ds.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var geoType = await conn.ExecuteScalarAsync<string>(sql.ToString());
            return geoType.Substring(3).ToLowerInvariant();
        }

        protected override AgsJsonParam ConvertQueryParams(DataSourceCacheItem ds, AgsQueryParam queryParam) {
            var param = new AgsJsonParam();

            return param;
        }

    }

}
