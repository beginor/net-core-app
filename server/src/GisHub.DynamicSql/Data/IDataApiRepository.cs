using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DynamicSql.Models;
using Beginor.GisHub.Geo.GeoJson;

namespace Beginor.GisHub.DynamicSql.Data {

    /// <summary>数据API 仓储接口</summary>
    public partial interface IDataApiRepository : IRepository<DataApiModel, long>, IRolesFilterProvider {

        /// <summary>搜索 数据API ，返回分页结果。</summary>
        Task<PaginatedResponseModel<DataApiModel>> SearchAsync(DataApiSearchModel model);

        Task SaveAsync(DataApiModel model, AppUser user, CancellationToken token = default);

        Task DeleteAsync(long id, AppUser user, CancellationToken token = default);

        Task UpdateAsync(long id, DataApiModel model, AppUser user, CancellationToken token = default);

        Task<IList<IDictionary<string, object>>> QueryAsync(DataApiCacheItem api, IDictionary<string, object> parameters);

        Task<IList<GeoJsonFeature>> QueryGeoJsonAsync(DataApiCacheItem api, IDictionary<string, object> parameters);

        Task<string> BuildSqlAsync(DataApiCacheItem api, IDictionary<string, object> parameters);

        Task<DataServiceFieldModel[]> GetColumnsAsync(DataApiCacheItem api, IDictionary<string, object> parameters);

        Task<DataApiCacheItem> GetDataApiCacheItemByIdAsync(long apiId);

        Task<IList<DataApiModel>> GetByIdArray(long[] idArray);

    }

}
