using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS; 

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
        var sql = "("
            + "  select t.schemaname as schema, t.tablename as name, pg_catalog.obj_description(c.oid) as description, 'BASE TABLE' as type"
            + "  from pg_catalog.pg_tables t"
            + "  left join pg_catalog.pg_class c on c.relname = t.tablename and c.relkind = 'r'"
            + "  where schemaname = @schema"
            + ") union all ("
            + "  select v.schemaname as schema, v.viewname as name, pg_catalog.obj_description(c.oid) as description, 'VIEW' as type"
            + "  from pg_catalog.pg_views v"
            + "  left join pg_catalog.pg_class c on c.relname = v.viewname and c.relkind = 'v'"
            + "  where schemaname = @schema"
            + " ) union all ("
            + "  select m.schemaname as schema, m.matviewname as name, pg_catalog.obj_description(c.oid) as description, 'MATERIALIZED VIEW' as type"
            + "  from pg_catalog.pg_matviews m"
            + "  left join pg_catalog.pg_class c on c.relname = m.matviewname and c.relkind = 'm'"
            + "  where m.schemaname = @schema"
            + ");";
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
        var sql = "select  n.nspname as schema, c.relname as \"table\", a.attname as name,"
            + "  pg_catalog.col_description(a.attrelid, a.attnum) as description,"
            + "  pg_catalog.format_type(a.atttypid, a.atttypmod) as type,"
            + "  case when a.attlen > 0 then a.attlen::integer else a.atttypmod - 4 end as length,"
            + "  not a.attnotnull as nullable"
            + " from pg_catalog.pg_attribute a"
            + " inner join pg_catalog.pg_class c on a.attrelid = c.oid"
            + " inner join pg_catalog.pg_namespace n on c.relnamespace = n.oid"
            + " where a.attnum > 0 and not a.attisdropped"
            + "  and n.nspname=@schema and c.relname = @tableName"
            + " order by a.attnum;";
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
