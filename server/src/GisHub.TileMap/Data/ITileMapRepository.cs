using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Data; 

/// <summary>切片地图 仓储接口</summary>
public partial interface ITileMapRepository : IRepository<TileMapModel, long> {

    /// <summary>搜索 切片地图 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<TileMapModel>> SearchAsync(
        TileMapSearchModel model
    );

    Task SaveAsync(
        TileMapModel model,
        AppUser user,
        CancellationToken token = default
    );

    Task UpdateAsync(
        long id,
        TileMapModel model,
        AppUser user,
        CancellationToken token = default
    );

    Task DeleteAsync(
        long id,
        AppUser user,
        CancellationToken token = default
    );

    Task<JsonElement> GetTileMapInfoAsync(long id);

    Task<TileContentModel> GetTileContentAsync(long id, int level, int row, int col);

    Task<DateTimeOffset?> GetTileModifiedTimeAsync(long id, int level, int row, int col);

}
