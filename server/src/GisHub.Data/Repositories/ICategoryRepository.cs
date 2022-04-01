using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>数据类别 仓储接口</summary>
public partial interface ICategoryRepository : IRepository<CategoryModel, long> {

    /// <summary>搜索 数据类别 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<CategoryModel>> SearchAsync(
        CategorySearchModel model
    );

}
