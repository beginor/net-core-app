using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Data;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IDataServiceReader {
        /// <summary>
        /// 读取数据服务的列信息
        /// </summary>
        Task<IList<ColumnModel>> GetColumnsAsync(DataServiceCacheItem dataService);
        /// <summary>
        /// 读取数据服务的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDataAsync(DataServiceCacheItem dataService, ReadDataParam param);
        /// <summary>
        /// 读取数据服务的记录数。
        /// </summary>
        Task<long> CountAsync(DataServiceCacheItem dataService, CountParam param);
        /// <summary>
        /// 读取数据服务中不重复的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(DataServiceCacheItem dataService, DistinctParam param);
        /// <summary>
        /// 行列转置数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> PivotData(DataServiceCacheItem dataService, PivotParam param);
        Task<T> ReadScalarAsync<T>(DataServiceCacheItem dataService, ReadDataParam param);
        DbConnection CreateConnection(string connectionString);
        Task<IList<IDictionary<string, object>>> ReadDataAsync(DbDataReader reader);
    }

}
