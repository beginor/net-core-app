using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Dapper;

namespace Beginor.GisHub.DataServices {

    public abstract class DataServiceReader : Disposable, IDataServiceReader {

        private ILogger<DataServiceReader> logger;

        protected IDataServiceFactory Factory { get; private set; }
        protected IDataServiceRepository DataServiceRepo { get; private set; }
        protected IConnectionRepository ConnectionRepo { get; private set; }

        protected DataServiceReader(
            IDataServiceFactory factory,
            IDataServiceRepository dataServiceRepo,
            IConnectionRepository connectionRepo,
            ILogger<DataServiceReader> logger
        ) {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            DataServiceRepo = dataServiceRepo ?? throw new ArgumentNullException(nameof(dataServiceRepo));
            ConnectionRepo = connectionRepo ?? throw new ArgumentNullException(nameof(connectionRepo));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                Factory = null;
                DataServiceRepo = null;
                ConnectionRepo = null;
                logger = null;
            }
            base.Dispose(disposing);
        }

        public Task<long> CountAsync(DataSourceCacheItem dataSource, CountParam param) {
            var sql = BuildCountSql(dataSource, param);
            return ReadScalarAsync<long>(dataSource, sql);
        }

        public virtual Task<IList<ColumnModel>> GetColumnsAsync(DataSourceCacheItem dataSource) {
            var columns = from field in dataSource.Fields
                select new ColumnModel {
                    Name = field.Name,
                    Description = field.Description,
                    Length = field.Length,
                    Nullable = field.Nullable,
                    Type = field.Type
                };
            IList<ColumnModel> result = columns.ToList();
            return Task.FromResult(result);
            // var dsModel = await DataSourceRepo.GetByIdAsync(dataSource.DataSourceId);
            // var connModel = await ConnectionRepo.GetByIdAsync(long.Parse(dsModel.Connection.Id));
            // var meta = Factory.CreateMetadataProvider(connModel.DatabaseType);
            // var columns = await meta.GetColumnsAsync(connModel, dataSource.Schema, dataSource.TableName);
            // return columns;
        }

        public virtual Task<IList<IDictionary<string, object>>> PivotData(DataSourceCacheItem dataSource, PivotParam param) {
            var sql = BuildPivotSql(dataSource, param);
            return ReadDataAsync(dataSource, sql);
        }

        public Task<IList<IDictionary<string, object>>> ReadDataAsync(
            DataSourceCacheItem dataSource,
            ReadDataParam param
        ) {
            var sql = BuildReadDataSql(dataSource, param);
            return ReadDataAsync(dataSource, sql);
        }

        public Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(DataSourceCacheItem dataSource, DistinctParam param) {
            var sql = BuildDistinctSql(dataSource, param);
            return ReadDataAsync(dataSource, sql);
        }

        public Task<T> ReadScalarAsync<T>(DataSourceCacheItem dataSource, ReadDataParam param) {
            var sql = BuildScalarSql(dataSource, param);
            return ReadScalarAsync<T>(dataSource, sql);
        }

        public abstract IDbConnection CreateConnection(DataSourceCacheItem dataSource);

        protected abstract string BuildReadDataSql(DataSourceCacheItem dataSource, ReadDataParam param);

        protected abstract string BuildCountSql(DataSourceCacheItem dataSource, CountParam param);

        protected abstract string BuildDistinctSql(DataSourceCacheItem dataSource, DistinctParam param);

        protected abstract string BuildPivotSql(DataSourceCacheItem dataSource, PivotParam param);

        protected abstract string BuildScalarSql(DataSourceCacheItem dataSource, ReadDataParam param);

        protected virtual KeyValuePair<string, object> ReadField(IDataReader dataReader, int fieldIndex) {
            var name = dataReader.GetName(fieldIndex);
            var value = dataReader.IsDBNull(fieldIndex) ? null : dataReader.GetValue(fieldIndex);
            return new KeyValuePair<string, object>(name, value);
        }

        protected virtual async Task<IList<IDictionary<string, object>>> ReadDataAsync(DataSourceCacheItem dataSource, string sql) {
            using var conn = CreateConnection(dataSource);
            logger.LogInformation(sql);
            var reader = await conn.ExecuteReaderAsync(sql);
            var result = new List<IDictionary<string, object>>();
            while (reader.Read()) {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++) {
                    var pair = ReadField(reader, i);
                    row.Add(pair.Key, pair.Value);
                }
                result.Add(row);
            }
            return result;
        }

        protected virtual async Task<T> ReadScalarAsync<T>(DataSourceCacheItem dataSource, string sql) {
            using var conn = CreateConnection(dataSource);
            logger.LogInformation(sql);
            var value = await conn.ExecuteScalarAsync<T>(sql);
            return value;
        }

        protected void AppendWhere(StringBuilder sql, string presetCriteria, string where) {
            if (presetCriteria.IsNotNullOrEmpty()) {
                sql.AppendLine($" where ({presetCriteria}) ");
                if (where.IsNotNullOrEmpty()) {
                    sql.Append($" and ({where}) ");
                }
            }
            else if (where.IsNotNullOrEmpty()) {
                sql.AppendLine($" where {where} ");
            }
        }

    }

}
