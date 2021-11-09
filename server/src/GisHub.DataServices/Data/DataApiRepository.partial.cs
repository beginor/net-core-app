using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DataServices.Data {

    partial class DataApiRepository {

        public async Task<IList<Dictionary<string, object>>> InvokeApiAsync(DataApiCacheItem cacheItem, IDictionary<string, object> parameters) {
            // check parameters;
            if (cacheItem == null) {
                throw new ArgumentNullException(nameof(cacheItem));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            // build sql first;
            var sql = dynamicSqlProvider.BuildDynamicSql(cacheItem.DatabaseType, cacheItem.Statement, parameters);
            if (sql.IsNullOrEmpty()) {
                throw new InvalidOperationException("Sql is empty!");
            }
            logger.LogInformation(sql);
            var factory = dynamicSqlProvider.GetDbProviderFactory(cacheItem.DatabaseType);
            await using var conn = factory.CreateConnection();
            conn.ConnectionString = cacheItem.ConnectionString;
            var reader = await conn.ExecuteReaderAsync(sql, parameters);
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync()) {
                var dict = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++) {
                    if (await reader.IsDBNullAsync(i)) {
                        continue;
                    }
                    var key = reader.GetName(i);
                    var val = reader.GetValue(i);
                    dict[key] = val;
                }
                result.Add(dict);
            }
            return result;
        }

        public async Task<DataApiCacheItem> GetCacheItemByIdAsync(long apiId) {
            var key = apiId.ToString();
            var cacheItem = await cache.GetAsync<DataApiCacheItem>(key);
            if (cacheItem != null) {
                return cacheItem;
            }
            var api = await Session.GetAsync<DataApi>(apiId);
            if (api == null) {
                return null;
            }
            cacheItem = api.ToCacheItem();
            var metaProvider = dataServiceFactory.CreateMetadataProvider(api.DataSource.DatabaseType);
            var model = Mapper.Map<DataSourceModel>(api.DataSource);
            cacheItem.ConnectionString = metaProvider.BuildConnectionString(model);
            await cache.SetAsync(key, cacheItem, commonOption.Cache.MemoryExpiration);
            return cacheItem;
        }

        public async Task<string> BuildSqlAsync(DataApiCacheItem cacheItem, IDictionary<string, object> parameters) {
            // check parameters;
            if (cacheItem == null) {
                throw new ArgumentNullException(nameof(cacheItem));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            // build sql and return;
            var sql = dynamicSqlProvider.BuildDynamicSql(
                cacheItem.DatabaseType,
                cacheItem.Statement,
                parameters
            );
            return sql;
        }

        public async Task<DataServiceFieldModel[]> GetColumnsAsync(DataApiCacheItem cacheItem, IDictionary<string, object> parameters) {
            // check parameters;
            if (cacheItem == null) {
                throw new ArgumentNullException(nameof(cacheItem));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            var sql = dynamicSqlProvider.BuildDynamicSql(cacheItem.DatabaseType, cacheItem.Statement, parameters);
            if (sql.IsNullOrEmpty()) {
                throw new InvalidOperationException("Sql is empty!");
            }
            logger.LogInformation(sql);
            var factory = dynamicSqlProvider.GetDbProviderFactory(cacheItem.DatabaseType);
            await using var conn = factory.CreateConnection();
            conn.ConnectionString = cacheItem.ConnectionString;
            var reader = await conn.ExecuteReaderAsync(sql, parameters);
            await reader.ReadAsync();
            var columns = new DataServiceFieldModel[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++) {
                columns[i] = new DataServiceFieldModel {
                    Name = reader.GetName(i),
                    Description = reader.GetName(i),
                    Editable = false,
                    Type = reader.GetDataTypeName(i)
                };
            }
            await reader.CloseAsync();
            return columns;
        }

    }

}
