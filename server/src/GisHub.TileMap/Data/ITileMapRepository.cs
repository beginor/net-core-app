using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>切片地图 仓储接口</summary>
    public partial interface ITileMapRepository : IRepository<TileMapModel, long> {

        /// <summary>搜索 切片地图 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<TileMapModel>> SearchAsync(
            TileMapSearchModel model
        );

        Task<JsonElement> GetTileMapInfoAsync(string tileName);

        Task<TileContentModel> GetTileContentAsync(string tileName, int level, int row, int col);

        Task<DateTimeOffset?> GetTileModifiedTimeAsync(string tileName, int level, int row, int col);

    }

}
