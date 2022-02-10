using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISMetaDataProvider : Disposable, IMetaDataProvider {

        public PostGISMetaDataProvider() { }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                // repo = null;
            }
            base.Dispose(disposing);
        }

        public string BuildConnectionString(DataSourceModel model) {
            var builder = new NpgsqlConnectionStringBuilder {
                ApplicationName = "GisHub",
                Host = model.ServerAddress,
                Port = model.ServerPort,
                Database = model.DatabaseName,
                Username = model.Username,
                Password = model.Password,
                CommandTimeout = model.Timeout,
                Timeout = model.Timeout
            };
            if (model.UseSsl) {
                builder.SslMode = SslMode.Require;
                builder.TrustServerCertificate = true;
            }
            else {
                builder.SslMode = SslMode.Disable;
            }
            return builder.ConnectionString;
        }

        public async Task GetStatusAsync(DataSourceModel model) {
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            await conn.ExecuteScalarAsync<DateTime>("select now();");
        }

        public async Task<IList<string>> GetSchemasAsync(DataSourceModel model) {
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = "select distinct t.table_schema from information_schema.tables t;";
            var schemes = await conn.QueryAsync<string>(sql);
            return schemes.ToList();
        }

        public async Task<IList<TableModel>> GetTablesAsync(
            DataSourceModel model,
            string schema
        ) {
            var schemas = await GetSchemasAsync(model);
            if (!schemas.Contains(schema)) {
                return new List<TableModel>(0);
            }
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = ""
                + " select"
                + " t.table_schema as schema,"
                + " t.table_name as name,"
                + " obj_description((t.table_schema||'.'||t.table_name)::regclass::oid) as description,"
                + " t.table_type as type"
                + " from information_schema.tables t"
                + " where t.table_schema = @schema"
                + " order by t.table_name";
            var meta = await conn.QueryAsync<TableModel>(sql, new {schema});
            return meta.ToList();
        }

        public async Task<IList<ColumnModel>> GetColumnsAsync(
            DataSourceModel model,
            string schema,
            string tableName
        ) {
            var schemas = await GetSchemasAsync(model);
            if (!schemas.Contains(schema)) {
                return new List<ColumnModel>(0);
            }
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = ""
                + " select"
                + " col.table_schema as schema,"
                + " col.table_name as table,"
                + " col.column_name as name,"
                + " col_description((col.table_schema || '.' || col.table_name)::regclass::oid, col.ordinal_position) as description,"
                + " col.udt_name as type,"
                + " coalesce(col.character_maximum_length, col.numeric_precision, 0) as length,"
                + " case col.is_nullable when 'YES' then true else false end as nullable"
                + " from information_schema.columns col"
                + " where col.table_catalog = @database "
                + " and col.table_schema = @schema"
                + " and col.table_name = @tableName"
                + " order by col.ordinal_position ";
            var columns = await conn.QueryAsync<ColumnModel>(
                sql,
                new {
                    database = model.DatabaseName,
                    schema,
                    tableName
                }
            );
            return columns.ToList();
        }

        public string GetDefaultSchema() => "public";

    }

}
