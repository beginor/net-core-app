using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices {

    public interface IDataSourceReader {
        /// <summary>
        /// 读取数据源的列信息
        /// </summary>
        Task<IList<ColumnModel>> GetColumnsAsync(
            long dataSourceId
        );
        /// <summary>
        /// 读取数据源的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDataAsync(
            long dataSourceId,
            string select,
            string where,
            string groupBy,
            string orderBy,
            int skip,
            int count
        );
        /// <summary>
        /// 读取数据源的记录数。
        /// </summary>
        Task<long> CountAsync(long dataSourceId, string where);
        /// <summary>
        /// 读取数据源中不重复的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(
            long dataSourceId,
            string select,
            string where,
            string orderBy
        );
        /// <summary>
        /// 行列转置数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> PivotData(
            long dataSourceId,
            string select,
            string where,
            string aggregate,
            string pivotField,
            string pivotValue,
            string orderBy
        );
    }

}
