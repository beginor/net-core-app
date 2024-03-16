using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>组织单元 仓储接口</summary>
public partial interface IAppOrganizeUnitRepository : IRepository<AppOrganizeUnitModel, long> {

    /// <summary>搜索 组织单元 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<AppOrganizeUnitModel>> SearchAsync(AppOrganizeUnitSearchModel model, ClaimsPrincipal user);

    Task<AppOrganizeUnit> GetDefaultAsync(CancellationToken token = default);

    Task<AppOrganizeUnitModel> GetByIdAsync(long id, ClaimsPrincipal user, CancellationToken token = default);

    Task<AppOrganizeUnit> GetEntityByIdAsync(long unitId, ClaimsPrincipal user, CancellationToken token = default);

    Task SaveAsync(AppOrganizeUnitModel model, ClaimsPrincipal user, CancellationToken token = default);

    Task UpdateAsync(long id, AppOrganizeUnitModel model, ClaimsPrincipal user, CancellationToken token = default);

    Task DeleteAsync(long id, ClaimsPrincipal user, CancellationToken token = default);

    Task<IList<AppOrganizeUnitModel>> QueryPathAsync(long unitId, CancellationToken token = default);

    Task<bool> CanViewOrganizeUnitAsync(long userUnitId, long unitId, CancellationToken token = default);

    Task CheckOrganizeUnitAsync(AppOrganizeUnit unit, ClaimsPrincipal user, CancellationToken token = default);

}
