using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISDataSourceReader : DataSourceReader {

        private ILogger<PostGISDataSourceReader> logger;

        public PostGISDataSourceReader(
            IDataServiceFactory factory,
            IDataSourceRepository dataSourceRepo,
            IConnectionRepository connectionRepo,
            ILogger<PostGISDataSourceReader> logger
        ) : base(factory, dataSourceRepo, connectionRepo) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                logger = null;
            }
            base.Dispose(disposing);
        }

        public override async Task<long> CountAsync(long dataSourceId, string where) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            var sb = new StringBuilder(" select count(*) ");
            sb.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sb, ds.PresetCriteria, where);
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var sql = sb.ToString();
            logger.LogInformation(sql);
            var count = await conn.ExecuteScalarAsync<long>(sql);
            return count;
        }

        public override async Task<IList<IDictionary<string, object>>> PivotData(
            long dataSourceId,
            string select,
            string where,
            string aggregate,
            string pivotField,
            string pivotValue,
            string orderBy
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (select.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(select)} can not be empty!");
            }
            if (aggregate.IsNullOrEmpty() || pivotField.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(pivotField)} and {nameof(aggregate)} can not be empty");
            }
            var cols = await GetColumnsAsync(dataSourceId);
            var colsDict = cols.ToDictionary(
                c => c.Name,
                c => c.Type,
                StringComparer.OrdinalIgnoreCase
            );
            var selectArray = select.Split(',').Select(t => t.ToLower().Trim()).ToArray();
            var newSelect = new List<string>();
            var aggColumn = "";
            var groupBy = "";
            foreach (var item in selectArray) {
                if (aggregate.ToLower().Contains($"({item})")) {
                    aggColumn = item;
                    newSelect.Add(aggregate);
                }
                else {
                    newSelect.Add(item);
                }
            }
            if (aggColumn.IsNotNullOrEmpty()) {
                var aggregate1 = aggregate;
                groupBy = " group by " + string.Join(",", newSelect.Where(t => !t.Equals(aggregate1)));
                select = string.Join(",", newSelect);
                aggregate = aggColumn;
            }

            if (!colsDict.ContainsKey(aggregate)) {
                throw new ArgumentNullException($"{nameof(aggregate)} not exists;");
            }
            if (!colsDict.ContainsKey(pivotField)) {
                throw new ArgumentNullException($"{nameof(pivotField)} not exists;");
            }
            var pivotValues = pivotValue.Split(',').Select(t => t.Trim());
            var pivotString = "";
            var valueString = "";
            foreach (var value in pivotValues) {
                if (!value.ToLower().Equals(pivotField) && !value.ToLower().Equals(aggregate)) {
                    if (selectArray.Contains(value.ToLower())) {
                        pivotString += $"\"{value}\" {colsDict[value.ToLower()]},";
                    }
                    else {
                        pivotString += $"\"{value}\" {colsDict[aggregate]},"; //提供aggregate给定字段的类型
                        valueString += $"('{value}'),";
                    }
                }
            }
            pivotString = pivotString.TrimEnd(',');
            if (valueString.IsNotNullOrEmpty()) {
                valueString = $", $$VALUES{valueString.TrimEnd(',')}$$";
            }

            var sql = new StringBuilder($"select * from crosstab('select {select} from {ds.Schema}.{ds.TableName} {groupBy} order by 1,2' {valueString}) as t ({pivotString})");
            AppendWhere(sql, ds.PresetCriteria, where);
            if (orderBy.IsNotNullOrEmpty()) {
                sql.Append($" order by {orderBy} ");
            }
            var result = await ReadDataAsync(dataSourceId, sql.ToString());
            return result;
        }

        public override async Task<IList<IDictionary<string, object>>> ReadDataAsync(
            long dataSourceId,
            string select,
            string where,
            string groupBy,
            string orderBy,
            int skip,
            int count
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (select.IsNullOrEmpty()) {
                select = $"{ds.PrimaryKeyColumn}, {ds.DisplayColumn}";
            }
            var sql = new StringBuilder();
            // check selected fields when a table has geo column;
            if (ds.HasGeometryColumn) {
                select = select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(ds.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select {select} ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sql, ds.PresetCriteria, where);
            if (groupBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" group by {groupBy} ");
                if (orderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {orderBy} ");
                }
            }
            else {
                if (orderBy.IsNullOrEmpty()) {
                    orderBy = ds.DefaultOrder;
                }
                if (orderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {orderBy} ");
                }
            }
            sql.AppendLine($" limit {count} offset {skip} ");
            var result = await ReadDataAsync(dataSourceId, sql.ToString());
            return result;
        }

        public override async Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(
            long dataSourceId,
            string select,
            string where,
            string orderBy
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (select.IsNullOrEmpty() || select.Trim().Equals("*")) {
                throw new ArgumentException($"Invalid select {select} for distinct !");
            }
            var sql = new StringBuilder();
            if (ds.HasGeometryColumn) {
                select = select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(ds.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select distinct {select} ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sql, ds.PresetCriteria, where);
            if (orderBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" order by {orderBy} ");
            }
            var data = await ReadDataAsync(dataSourceId, sql.ToString());
            return data;
        }

        private async Task<IList<IDictionary<string, object>>> ReadDataAsync(
            long dataSourceId,
            string sql
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId}");
            }
            logger.LogInformation(sql);
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            await conn.OpenAsync();
            var tableData = await ReadDataAsync(conn, sql);
            await conn.CloseAsync();
            return tableData;
        }

    }

}
