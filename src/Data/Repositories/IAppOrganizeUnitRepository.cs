using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>组织单元 仓储接口</summary>
public partial interface IAppOrganizeUnitRepository : IRepository<AppOrganizeUnitModel, long> {

    /// <summary>搜索 组织单元 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<AppOrganizeUnitModel>> SearchAsync(
        AppOrganizeUnitSearchModel model
    );

    Task SaveAsync(AppOrganizeUnitModel model, string userName);

    Task UpdateAsync(long id, AppOrganizeUnitModel model, string userName);

}
