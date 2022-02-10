using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.Geo.GeoJson;
using Dapper;

namespace Beginor.GisHub.DynamicSql.Data {

    partial class DataApiRepository {

        public async Task<IList<IDictionary<string, object>>> QueryAsync(DataApiCacheItem api, IDictionary<string, object> parameters) {
            // check parameters;
            if (api == null) {
                throw new ArgumentNullException(nameof(api));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            // build sql first;
            var sql = dynamicSqlProvider.BuildDynamicSql(api.DatabaseType, api.Statement, parameters);
            if (sql.IsNullOrEmpty()) {
                throw new InvalidOperationException("Sql is empty!");
            }
            logger.LogInformation(sql);
            var dsReader = dataServiceFactory.CreateDataSourceReader(api.DatabaseType);
            var conn = dsReader.CreateConnection(api.ConnectionString);
            var reader = await conn.ExecuteReaderAsync(sql, parameters);
            var result = await dsReader.ReadDataAsync(reader);
            return result;
        }

        public async Task<IList<GeoJsonFeature>> QueryGeoJsonAsync(DataApiCacheItem api, IDictionary<string, object> parameters) {
            // check parameters;
            if (api == null) {
                throw new ArgumentNullException(nameof(api));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            // build sql first;
            var sql = dynamicSqlProvider.BuildDynamicSql(api.DatabaseType, api.Statement, parameters);
            if (sql.IsNullOrEmpty()) {
                throw new InvalidOperationException("Sql is empty!");
            }
            logger.LogInformation(sql);
            var dsReader = dataServiceFactory.CreateDataSourceReader(api.DatabaseType);
            var conn = dsReader.CreateConnection(api.ConnectionString);
            var reader = await conn.ExecuteReaderAsync(sql, parameters);
            var data = await dsReader.ReadDataAsync(reader);
            var featureReader = dataServiceFactory.CreateFeatureProvider(api.DatabaseType);
            var features = featureReader.ConvertToGeoJson(data, api.IdColumn, api.GeometryColumn);
            return features;
        }

        public async Task<DataApiCacheItem> GetDataApiCacheItemByIdAsync(long apiId) {
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

        public Task<string> BuildSqlAsync(DataApiCacheItem cacheItem, IDictionary<string, object> parameters) {
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
            return Task.FromResult(sql);
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
            var tables = FindTableNames(sql);
            var api = await Session.GetAsync<DataApi>(cacheItem.DataApiId);
            var dataSource = Mapper.Map<DataSourceModel>(api.DataSource);
            var dbColumns = await GetColumnsFromDataSourceAsync(dataSource, tables);
            //
            var factory = dynamicSqlProvider.GetDbProviderFactory(cacheItem.DatabaseType);
            await using var conn = factory.CreateConnection();
            conn.ConnectionString = cacheItem.ConnectionString;
            var reader = await conn.ExecuteReaderAsync(sql, parameters);
            await reader.ReadAsync();
            var columns = new DataServiceFieldModel[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++) {
                var name = reader.GetName(i);
                var descs = dbColumns
                    .Where(col => col.Name.EqualsOrdinalIgnoreCase(name))
                    .Select(col => col.Description).ToArray();
                columns[i] = new DataServiceFieldModel {
                    Name = name,
                    Description = descs.Length > 0 ? string.Join(";", descs) : reader.GetName(i),
                    Editable = false,
                    Type = reader.GetDataTypeName(i)
                };
            }
            await reader.CloseAsync();
            return columns;
        }

        private IList<string> FindTableNames(string sql) {
            var regex = new Regex(@"(?<=(from|join)\s+)(\w*\.\w*|\w*)");
            return regex.Matches(sql).Select(match => match.Value).ToList();
        }

        private async Task<IList<DataServiceFieldModel>> GetColumnsFromDataSourceAsync(DataSourceModel dataSource, IList<string> tableNames) {
            var metadata = dataServiceFactory.CreateMetadataProvider(dataSource.DatabaseType);
            var result = new List<DataServiceFieldModel>();
            foreach (var tableName in tableNames) {
                var parts = tableName.Split('.');
                string schema;
                string table;
                if (parts.Length == 0) {
                    schema = metadata.GetDefaultSchema();
                    table = parts[0];
                }
                else {
                    schema = parts[0];
                    table = parts[1];
                }
                var columns = await metadata.GetColumnsAsync(dataSource, schema, table);
                var fields = columns.Select(col => new DataServiceFieldModel {
                    Name = col.Name,
                    Description = col.Description,
                    Type = col.Type,
                    Length = col.Length,
                    Nullable = col.Nullable,
                    Editable = false
                });
                result.AddRange(fields);
            }
            return result;
        }

    }

}
