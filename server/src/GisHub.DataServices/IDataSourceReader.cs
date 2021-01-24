using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.GisHub.DataServices.GeoJson;
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
            ReadDataParam param
        );
        /// <summary>
        /// 读取数据源的记录数。
        /// </summary>
        Task<long> CountAsync(long dataSourceId, CountParam param);
        /// <summary>
        /// 读取数据源中不重复的数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> ReadDistinctDataAsync(
            long dataSourceId,
            DistinctParam param
        );
        /// <summary>
        /// 行列转置数据
        /// </summary>
        Task<IList<IDictionary<string, object>>> PivotData(
            long dataSourceId,
            PivotParam param
        );

        /// <summary>
        /// Read data as GeoJson Feature Collection
        /// </summary>
        Task<GeoJsonFeatureCollection> ReadAsFeatureCollection(
            long dataSourceId,
            GeoJsonParam param
        );

    }

}
