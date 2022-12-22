using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>json 数据 仓储接口</summary>
public partial interface IAppJsonDataRepository : IRepository<AppJsonDataModel, long> {

    /// <summary>搜索 json 数据 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<AppJsonDataModel>> SearchAsync(
        AppJsonDataSearchModel model
    );

    Task<JsonElement> GetValueByIdAsync(long id);

    Task SaveValueAsync(long id, JsonElement value);

}
