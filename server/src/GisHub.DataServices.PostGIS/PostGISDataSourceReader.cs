using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.PostGIS {

    public class PostGISDataSourceReader : DataSourceReader {

        // private ILogger<PostGISDataSourceReader> logger;

        public PostGISDataSourceReader(
            IDataServiceFactory factory,
            IDataSourceRepository dataSourceRepo,
            IConnectionRepository connectionRepo,
            ILogger<PostGISDataSourceReader> logger
        ) : base(factory, dataSourceRepo, connectionRepo, logger) {
            // this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                // logger = null;
            }
            base.Dispose(disposing);
        }

        protected override string BuildPivotSql(DataSourceCacheItem dataSource, PivotParam param) {
            if (param.Select.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(param.Select)} can not be empty!");
            }
            if (param.Aggregate.IsNullOrEmpty() || param.Field.IsNullOrEmpty()) {
                throw new ArgumentNullException($"{nameof(param.Field)} and {nameof(param.Aggregate)} can not be empty");
            }
            var task = GetColumnsAsync(dataSource);
            task.Wait();
            var cols = task.Result;
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

            var sql = new StringBuilder($"select * from crosstab('select {param.Select} from {dataSource.Schema}.{dataSource.TableName} {groupBy} order by 1,2' {valueString}) as t ({pivotString})");
            AppendWhere(sql, dataSource.PresetCriteria, param.Where);
            if (param.OrderBy.IsNotNullOrEmpty()) {
                sql.Append($" order by {param.OrderBy} ");
            }
            return sql.ToString();
        }

        public override IDbConnection CreateConnection(DataSourceCacheItem dataSource) {
            return new NpgsqlConnection(dataSource.ConnectionString);
        }

        protected override string BuildReadDataSql(DataSourceCacheItem dataSource, ReadDataParam param) {
            if (param.Select.IsNullOrEmpty()) {
                param.Select = $"{dataSource.PrimaryKeyColumn}, {dataSource.DisplayColumn}";
            }
            var sql = new StringBuilder();
            // check selected fields when a table has geo column;
            if (param.CheckGeometryColumn && dataSource.HasGeometryColumn) {
                param.Select = param.Select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(dataSource.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select {param.Select} ");
            sql.AppendLine($" from {dataSource.Schema}.{dataSource.TableName} ");
            AppendWhere(sql, dataSource.PresetCriteria, param.Where);
            if (param.GroupBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" group by {param.GroupBy} ");
                if (param.OrderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {param.OrderBy} ");
                }
            }
            else {
                if (param.OrderBy.IsNullOrEmpty()) {
                    param.OrderBy = dataSource.DefaultOrder;
                }
                if (param.OrderBy.IsNotNullOrEmpty()) {
                    sql.AppendLine($" order by {param.OrderBy} ");
                }
            }
            sql.AppendLine($" limit {param.Take} offset {param.Skip} ");
            return sql.ToString();
        }

        protected override string BuildCountSql(DataSourceCacheItem dataSource, CountParam param) {
            var sql = new StringBuilder(" select count(*) ");
            sql.AppendLine($" from {dataSource.Schema}.{dataSource.TableName} ");
            AppendWhere(sql, dataSource.PresetCriteria, param.Where);
            return sql.ToString();
        }

        protected override string BuildDistinctSql(DataSourceCacheItem dataSource, DistinctParam param) {
            if (param.Select.IsNullOrEmpty() || param.Select.Trim().Equals("*")) {
                throw new ArgumentException($"Invalid select {param.Select} for distinct !");
            }
            var sql = new StringBuilder();
            if (dataSource.HasGeometryColumn) {
                param.Select = param.Select.Split(
                    ",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
                ).Aggregate(
                    new StringBuilder(),
                    (current, field) => current.Append(",").Append(field.EqualsOrdinalIgnoreCase(dataSource.GeometryColumn) ? $"st_astext({field}) as {field}" : field)
                ).ToString().Substring(1);
            }
            sql.AppendLine($" select distinct {param.Select} ");
            sql.AppendLine($" from {dataSource.Schema}.{dataSource.TableName} ");
            AppendWhere(sql, dataSource.PresetCriteria, param.Where);
            if (param.OrderBy.IsNotNullOrEmpty()) {
                sql.AppendLine($" order by {param.OrderBy} ");
            }
            return sql.ToString();
        }
    }

}
