using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.DataServices.Data {

    partial class DataApiRepository {

        public async Task<IList<Dictionary<string, object>>> InvokeApi(long apiId, IDictionary<string, object> parameters) {
            var cacheItem = await GetCacheItemByIdAsync(apiId);
            if (cacheItem == null) {
                return null;
            }
            var sql = dynamicSqlProvider.BuildDynamicSql(
                cacheItem.DatabaseType,
                cacheItem.Statement,
                parameters
            );
            if (sql.IsNullOrEmpty()) {
                throw new InvalidOperationException("Sql is empty!");
            }
            var factory = dynamicSqlProvider.GetDbProviderFactory(cacheItem.DatabaseType);
            var conn = factory.CreateConnection();
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
            await cache.SetAsync(key, cacheItem, commonOption.Cache.MemoryExpiration);
            return cacheItem;
        }

    }

}
