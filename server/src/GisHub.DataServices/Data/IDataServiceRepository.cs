using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据服务 仓储接口</summary>
    public partial interface IDataServiceRepository : IRepository<DataServiceModel, long> {

        /// <summary>搜索 数据服务 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<DataServiceModel>> SearchAsync(
            DataSourceSearchModel model,
            string[] roles
        );

        Task<DataSourceCacheItem> GetCacheItemByIdAsync(
            long id
        );

    }

}
