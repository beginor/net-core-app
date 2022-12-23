using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;
using Dapper;

namespace Beginor.GisHub.DataServices;

public abstract class DataServiceReader : Disposable, IDataServiceReader {

    private ILogger<DataServiceReader> logger;

    protected IDataServiceFactory Factory { get; private set; }
    protected IDataServiceRepository DataServiceRepo { get; private set; }
    protected IDataSourceRepository DataSourceRepo { get; private set; }

    protected DataServiceReader(
        IDataServiceFactory factory,
        IDataServiceRepository dataServiceRepo,
        IDataSourceRepository dataSourceRepo,
        ILogger<DataServiceReader> logger
    ) {
        Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        DataServiceRepo = dataServiceRepo ?? throw new ArgumentNullException(nameof(dataServiceRepo));
        DataSourceRepo = dataSourceRepo ?? throw new ArgumentNullException(nameof(dataSourceRepo));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override void Dispose(
        bool disposing
    ) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    public Task<long> CountAsync(DataServiceCacheItem dataService, CountParam param) {
        var sql = BuildCountSql(dataService, param);
        return ReadScalarAsync<long>(dataService, sql);
    }

    public virtual Task<IList<ColumnModel>> GetColumnsAsync(DataServiceCacheItem dataService) {
        var columns = from field in dataService.Fields
            select new ColumnModel {
                Name = field.Name,
                Description = field.Description,
                Length = field.Length,
                Nullable = field.Nullable,
                Type = field.Type
            };
        IList<ColumnModel> result = columns.ToList();
        return Task.FromResult(result);
    }

    public virtual Task<IList<IDictionary<string, object?>>> PivotData(DataServiceCacheItem dataService, PivotParam param) {
        var sql = BuildPivotSql(dataService, param);
        return ReadDataAsync(dataService, sql);
    }

    public Task<IList<IDictionary<string, object?>>> ReadDataAsync(
        DataServiceCacheItem dataService,
        ReadDataParam param
    ) {
        var sql = BuildReadDataSql(dataService, param);
        return ReadDataAsync(dataService, sql);
    }

    public Task<IList<IDictionary<string, object?>>> ReadDistinctDataAsync(DataServiceCacheItem dataService, DistinctParam param) {
        var sql = BuildDistinctSql(dataService, param);
        return ReadDataAsync(dataService, sql);
    }

    public Task<T> ReadScalarAsync<T>(DataServiceCacheItem dataService, ReadDataParam param) {
        var sql = BuildScalarSql(dataService, param);
        return ReadScalarAsync<T>(dataService, sql);
    }

    public async Task<IList<IDictionary<string, object?>>> ReadDataAsync(DbDataReader reader) {
        var result = new List<IDictionary<string, object?>>();
        while (await reader.ReadAsync()) {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++) {
                var (key, value) = ReadField(reader, i);
                row.Add(key, value);
            }
            result.Add(row);
        }
        return result;
    }

    public abstract DbConnection CreateConnection(string connectionString);

    protected abstract string BuildReadDataSql(DataServiceCacheItem dataService, ReadDataParam param);

    protected abstract string BuildCountSql(DataServiceCacheItem dataService, CountParam param);

    protected abstract string BuildDistinctSql(DataServiceCacheItem dataService, DistinctParam param);

    protected abstract string BuildPivotSql(DataServiceCacheItem dataService, PivotParam param);

    protected abstract string BuildScalarSql(DataServiceCacheItem dataService, ReadDataParam param);

    protected virtual KeyValuePair<string, object?> ReadField(IDataReader dataReader, int fieldIndex) {
        var name = dataReader.GetName(fieldIndex);
        var value = dataReader.IsDBNull(fieldIndex) ? null : dataReader.GetValue(fieldIndex);
        return new KeyValuePair<string, object?>(name, value);
    }

    protected virtual async Task<IList<IDictionary<string, object?>>> ReadDataAsync(DataServiceCacheItem dataService, string sql) {
        await using var conn = CreateConnection(dataService.ConnectionString);
        logger.LogInformation(sql);
        var reader = await conn.ExecuteReaderAsync(sql);
        var result = await ReadDataAsync(reader);
        return result;
    }

    protected virtual async Task<T> ReadScalarAsync<T>(DataServiceCacheItem dataService, string sql) {
        await using var conn = CreateConnection(dataService.ConnectionString);
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
