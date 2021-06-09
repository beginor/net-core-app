using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IDataServiceReader {
        /// <summary>
        /// 读取数据服务的列信息
        /// </summary>
        Task<IList<ColumnModel>> GetColumnsAsync(DataSourceCacheItem dataSource);
        /// <summary>
        /// 读取数据服务的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDataAsync(DataSourceCacheItem dataSource, ReadDataParam param);
        /// <summary>
        /// 读取数据服务的记录数。
        /// </summary>
        Task<long> CountAsync(DataSourceCacheItem dataSource, CountParam param);
        /// <summary>
        /// 读取数据服务中不重复的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(DataSourceCacheItem dataSource, DistinctParam param);
        /// <summary>
        /// 行列转置数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> PivotData(DataSourceCacheItem dataSource, PivotParam param);
        Task<T> ReadScalarAsync<T>(DataSourceCacheItem dataSource, ReadDataParam param);
        IDbConnection CreateConnection(DataSourceCacheItem dataSource);
    }

}
