using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using MySql.Data.MySqlClient;

namespace Beginor.GisHub.DataServices.MySql;

public class MySqlMetaDataProvider : Disposable, IMetaDataProvider {

    public string BuildConnectionString(DataSourceModel model) {
        var builder = new MySqlConnectionStringBuilder {
            Server = model.ServerAddress,
            Port = (uint)model.ServerPort,
            Database = model.DatabaseName,
            UserID = model.Username,
            Password = model.Password,
            ConnectionTimeout = (uint)model.Timeout,
            DefaultCommandTimeout = (uint)model.Timeout,
            AllowUserVariables = true,
            CharacterSet = "utf8"
        };
        builder.SslMode = model.UseSsl ? MySqlSslMode.Required : MySqlSslMode.Disabled;
        return builder.ConnectionString;
    }

    public async Task GetStatusAsync(DataSourceModel model) {
        var connStr = BuildConnectionString(model);
        await using var conn = new MySqlConnection(connStr);
        await conn.ExecuteScalarAsync<DateTime>("select now();");
    }

    public Task<IList<string>> GetSchemasAsync(DataSourceModel model) {
        throw new NotSupportedException("MySQL does not database schema");
    }

    public async Task<IList<TableModel>> GetTablesAsync(DataSourceModel model, string schema) {
        var connStr = BuildConnectionString(model);
        await using var conn = new MySqlConnection(connStr);
        var sql = ""
            + " select"
            + " t.table_schema as `schema`,"
            + " t.table_name as `name`,"
            + " t.table_comment as `description`,"
            + " t.table_type as `type`"
            + " from information_schema.tables t"
            + " where t.table_type <> 'SYSTEM VIEW'"
            + " and t.table_schema = @database"
            + " order by t.table_name";
        var meta = await conn.QueryAsync<TableModel>(
            sql,
            new {
                database = model.DatabaseName
            }
        );
        return meta.ToList();
    }

    public async Task<IList<ColumnModel>> GetColumnsAsync(DataSourceModel model, string schema, string tableName) {
        var connStr = BuildConnectionString(model);
        await using var conn = new MySqlConnection(connStr);
        var sql = ""
            + " select"
            + " col.TABLE_SCHEMA as `schema`,"
            + " col.table_name as `table`,"
            + " col.column_name as `name`,"
            + " col.column_comment as `description`,"
            + " col.data_type as `type`,"
            + " (case when col.numeric_precision is not null then (col.numeric_precision+1)"
            + " when col.character_maximum_length is not null then col.character_maximum_length"
            + " else null end) as `length`,"
            + " ( case when col.is_nullable = 'no' then 0 else 1 end ) `nullable`"
            + " from information_schema.columns as col"
            + " where col.table_schema= @database"
            + " and col.table_name=@tableName"
            + " order by col.ordinal_position asc";
        var columns = await conn.QueryAsync<ColumnModel>(
            sql,
            new {
                database = model.DatabaseName,
                tableName
            }
        );
        return columns.ToList();
    }

    public string GetDefaultSchema() => string.Empty;

}
