using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Dapper;

namespace Beginor.GisHub.DataServices {

    public abstract class DataSourceReader : Disposable, IDataSourceReader {

        protected IDataServiceFactory Factory { get; private set; }
        protected IDataSourceRepository DataSourceRepo { get; private set; }
        protected IConnectionRepository ConnectionRepo { get; private set; }

        protected DataSourceReader(
            IDataServiceFactory factory,
            IDataSourceRepository dataSourceRepo,
            IConnectionRepository connectionRepo
        ) {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            DataSourceRepo = dataSourceRepo ?? throw new ArgumentNullException(nameof(dataSourceRepo));
            ConnectionRepo = connectionRepo ?? throw new ArgumentNullException(nameof(connectionRepo));
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                Factory = null;
                DataSourceRepo = null;
                ConnectionRepo = null;
            }
            base.Dispose(disposing);
        }

        public abstract Task<long> CountAsync(DataSourceCacheItem dataSource, CountParam param);

        public virtual async Task<IList<ColumnModel>> GetColumnsAsync(DataSourceCacheItem dataSource) {
            var dsModel = await DataSourceRepo.GetByIdAsync(dataSource.DataSourceId);
            var connModel = await ConnectionRepo.GetByIdAsync(long.Parse(dsModel.Connection.Id));
            var meta = Factory.CreateMetadataProvider(connModel.DatabaseType);
            var columns = await meta.GetColumnsAsync(connModel, dataSource.Schema, dataSource.TableName);
            return columns;
        }

        public abstract Task<IList<IDictionary<string, object>>> PivotData(DataSourceCacheItem dataSource, PivotParam param);

        public abstract Task<IList<IDictionary<string, object>>> ReadDataAsync(DataSourceCacheItem dataSource, ReadDataParam param);

        public abstract Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(DataSourceCacheItem dataSource, DistinctParam param);

        protected virtual KeyValuePair<string, object> ReadField(
            IDataReader dataReader,
            int fieldIndex
        ) {
            var name = dataReader.GetName(fieldIndex);
            var value = dataReader.GetValue(fieldIndex);
            return new KeyValuePair<string, object>(name, value);
        }
        protected virtual async Task<IList<IDictionary<string, object>>> ReadDataAsync(
            IDbConnection conn,
            string sql
        ) {
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

        protected void AppendWhere(
            StringBuilder sql,
            string presetCriteria,
            string where
        ) {
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
