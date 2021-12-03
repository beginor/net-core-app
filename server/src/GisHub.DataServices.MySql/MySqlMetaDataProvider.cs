using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using MySql.Data.MySqlClient;

namespace Beginor.GisHub.DataServices.MySql {

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
            if (model.UseSsl) {
                builder.SslMode = MySqlSslMode.Required;
            }
            return builder.ConnectionString;
        }

        public Task GetStatus(DataSourceModel model) {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetSchemasAsync(DataSourceModel model) {
            throw new NotImplementedException();
        }

        public Task<IList<TableModel>> GetTablesAsync(DataSourceModel model, string schema
        ) {
            throw new NotImplementedException();
        }

        public Task<IList<ColumnModel>> GetColumnsAsync(DataSourceModel model, string schema, string tableName) {
            throw new NotImplementedException();
        }

    }

}
