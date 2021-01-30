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

        public override async Task<long> CountAsync(long dataSourceId, CountParam param) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            var sb = new StringBuilder(" select count(*) ");
            sb.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sb, ds.PresetCriteria, param.Where);
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var sql = sb.ToString();
            logger.LogInformation(sql);
            var count = await conn.ExecuteScalarAsync<long>(sql);
            return count;
        }

        public override async Task<IList<IDictionary<string, object>>> PivotData(
            long dataSourceId,
            PivotParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (param.Select.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(param.Select)} can not be empty!");
            }
            if (param.Aggregate.IsNullOrEmpty() || param.Field.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(param.Field)} and {nameof(param.Aggregate)} can not be empty");
            }
            var cols = await GetColumnsAsync(dataSourceId);
            var colsDict = cols.ToDictionary(
                c => c.Name,
                c => c.Type,
                StringComparer.OrdinalIgnoreCase
            );
            var selectArray = param.Select.Split(',').Select(t => t.ToLower().Trim()).ToArray();
            var newSelect = new List<string>();
            var aggColumn = "";
            var groupBy = "";
            foreach (var item in selectArray) {
                if (param.Aggregate.ToLower().Contains($"({item})")) {
                    aggColumn = item;
                    newSelect.Add(param.Aggregate);
                }
                else {
                    newSelect.Add(item);
                }
            }
            if (aggColumn.IsNotNullOrEmpty()) {
                var aggregate1 = param.Aggregate;
                groupBy = " group by " + string.Join(",", newSelect.Where(t => !t.Equals(aggregate1)));
                param.Select = string.Join(",", newSelect);
                param.Aggregate = aggColumn;
            }

            if (!colsDict.ContainsKey(param.Aggregate)) {
                throw new ArgumentNullException($"{nameof(param.Aggregate)} not exists;");
            }
            if (!colsDict.ContainsKey(param.Field)) {
                throw new ArgumentNullException($"{nameof(param.Field)} not exists;");
            }
            var pivotValues = param.Value.Split(',').Select(t => t.Trim());
            var pivotString = "";
            var valueString = "";
            foreach (var value in pivotValues) {
                if (!value.ToLower().Equals(param.Field) && !value.ToLower().Equals(param.Aggregate)) {
                    if (selectArray.Contains(value.ToLower())) {
                        pivotString += $"\"{value}\" {colsDict[value.ToLower()]},";
                    }
                    else {
                        pivotString += $"\"{value}\" {colsDict[param.Aggregate]},"; //提供aggregate给定字段的类型
                        valueString += $"('{value}'),";
                    }
                }
            }
            pivotString = pivotString.TrimEnd(',');
            if (valueString.IsNotNullOrEmpty()) {
                valueString = $", $$VALUES{valueString.TrimEnd(',')}$$";
            }

            var sql = new StringBuilder($"select * from crosstab('select {param.Select} from {ds.Schema}.{ds.TableName} {groupBy} order by 1,2' {valueString}) as t ({pivotString})");
            AppendWhere(sql, ds.PresetCriteria, param.Where);
            if (param.OrderBy.IsNotNullOrEmpty()) {
                sql.Append($" order by {param.OrderBy} ");
            }
            var result = await ReadDataAsync(dataSourceId, sql.ToString());
            return result;
        }

        public override async Task<IList<IDictionary<string, object>>> ReadDataAsync(
            long dataSourceId,
            ReadDataParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (param.Select.IsNullOrEmpty()) {
                param.Select = $"{ds.PrimaryKeyColumn}, {ds.DisplayColumn}";
            }
            var sql = new StringBuilder();
            // check selected fields when a table has geo column;
            if (ds.HasGeometryColumn) {
                param.Select = param.Select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(ds.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select {param.Select} ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sql, ds.PresetCriteria, param.Where);
            if (param.GroupBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" group by {param.GroupBy} ");
                if (param.OrderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {param.OrderBy} ");
                }
            }
            else {
                if (param.OrderBy.IsNullOrEmpty()) {
                    param.OrderBy = ds.DefaultOrder;
                }
                if (param.OrderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {param.OrderBy} ");
                }
            }
            sql.AppendLine($" limit {param.Take} offset {param.Skip} ");
            var result = await ReadDataAsync(dataSourceId, sql.ToString());
            return result;
        }

        public override async Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(
            long dataSourceId,
            DistinctParam param
        ) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            if (param.Select.IsNullOrEmpty() || param.Select.Trim().Equals("*")) {
                throw new ArgumentException($"Invalid select {param.Select} for distinct !");
            }
            var sql = new StringBuilder();
            if (ds.HasGeometryColumn) {
                param.Select = param.Select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(ds.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select distinct {param.Select} ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            AppendWhere(sql, ds.PresetCriteria, param.Where);
            if (param.OrderBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" order by {param.OrderBy} ");
            }
            var data = await ReadDataAsync(dataSourceId, sql.ToString());
            return data;
        }

        public override async Task<int> GetSridAsync(long dataSourceId) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            var sql = new StringBuilder();
            sql.AppendLine($" select st_srid({ds.GeometryColumn}) ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            sql.AppendLine($" where {ds.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var srid = await conn.ExecuteScalarAsync<int>(sql.ToString());
            return srid;
        }

        public override async Task<string> GetGeometryTypeAsync(long dataSourceId) {
            var ds = await DataSourceRepo.GetCacheItemByIdAsync(dataSourceId);
            if (ds == null) {
                throw new ArgumentException($"Invalid dataSourceId {dataSourceId} !");
            }
            var sql = new StringBuilder();
            sql.AppendLine($" select st_geometrytype({ds.GeometryColumn}) ");
            sql.AppendLine($" from {ds.Schema}.{ds.TableName} ");
            sql.AppendLine($" where {ds.GeometryColumn} is not null ");
            sql.AppendLine($" limit 1 ;");
            await using var conn = new NpgsqlConnection(ds.ConnectionString);
            var geoType = await conn.ExecuteScalarAsync<string>(sql.ToString());
            return geoType.Substring(3).ToLowerInvariant();
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
