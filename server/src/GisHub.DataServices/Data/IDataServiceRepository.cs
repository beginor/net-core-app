using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data; 

/// <summary>数据服务 仓储接口</summary>
public partial interface IDataServiceRepository : IRepository<DataServiceModel, long>, IRolesFilterProvider {

    /// <summary>搜索 数据服务 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<DataServiceModel>> SearchAsync(
        DataServiceSearchModel model,
        string[] roles
    );

    Task<DataServiceCacheItem> GetCacheItemByIdAsync(
        long id
    );

}