using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISConnectionProvider : Disposable, IConnectionProvider {
        
        public PostGISConnectionProvider() { }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                // repo = null;
            }
            base.Dispose(disposing);
        }

        public string BuildConnectionString(ConnectionModel model) {
            var builder = new NpgsqlConnectionStringBuilder {
                ApplicationName = "GisHub",
                Host = model.ServerAddress,
                Port = model.ServerPort,
                Database = model.DatabaseName,
                Username = model.Username,
                Password = model.Password,
                CommandTimeout = model.Timeout
            };
            return builder.ConnectionString;
        }

        public async Task<IList<string>> GetSchemasAsync(ConnectionModel model) {
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = "select schema_name from information_schema.schemata"
                + " where schema_owner = @username and schemata.catalog_name = @databaseName;";
            var schemes = await conn.QueryAsync<string>(sql.ToString(), model);
            return schemes.ToList();
        }

        public async Task<IList<TableModel>> GetTablesAsync(
            ConnectionModel model,
            string schema
        ) {
            var schemas = await GetSchemasAsync(model);
            if (!schemas.Contains(schema)) {
                return new List<TableModel>(0);
            }
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = "select t.tablename as table_name, obj_description(c.OID) as description, 'table' as type"
                + " from pg_catalog.pg_tables t"
                + " left join pg_catalog.pg_class c on c.relname = t.tablename and c.relkind = 'r'"
                + " where t.schemaname = @schema"
                + " union all"
                + " select v.viewname as table_name, obj_description(c.OID) as description, 'view' as type"
                + " from pg_catalog.pg_views v"
                + " left join pg_catalog.pg_class c on c.relname = v.viewname and c.relkind = 'v'"
                + " where v.schemaname = @schema";
            var meta = await conn.QueryAsync<TableModel>(sql, new {schema});
            return meta.ToList();
        }

        public async Task<IList<ColumnModel>> GetColumnsAsync(
            ConnectionModel model,
            string schema,
            string tableName
        ) {
            var schemas = await GetSchemasAsync(model);
            if (!schemas.Contains(schema)) {
                return new List<ColumnModel>(0);
            }
            var connStr = BuildConnectionString(model);
            await using var conn = new NpgsqlConnection(connStr);
            var sql = "select"
                + " col.column_name,"
                + " col_description((col.table_schema || '.' || col.table_name)::regclass::oid, col.ordinal_position) as description,"
                + " col.udt_name as data_type,"
                + " coalesce(col.character_maximum_length, col.numeric_precision, 0) as length,"
                + " case col.is_nullable when 'YES' then true else false end as is_nullable"
                + " from information_schema.columns col"
                + " where col.table_catalog = @database "
                + " and col.table_schema = @schema"
                + " and col.table_name = @tableName";
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
        
    }

}
