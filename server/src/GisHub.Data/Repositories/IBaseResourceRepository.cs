using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>数据资源的基类 仓储接口</summary>
public partial interface IBaseResourceRepository {

    /// <summary>搜索 数据资源的基类 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<BaseResourceModel>> SearchAsync(
        BaseResourceSearchModel model
    );
    
    Task<PaginatedResponseModel<CategoryCountModel>> CountByCategoryAsync(BaseResourceStatisticRequestModel model);

}
