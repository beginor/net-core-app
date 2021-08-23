using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>矢量切片包 仓储接口</summary>
    public partial interface IVectorTileRepository : IRepository<VectorTileModel, long> {

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<VectorTileModel>> SearchAsync(
            VectorTileSearchModel model
        );

        Task SaveAsync(
            VectorTileModel model,
            string userId,
            CancellationToken token = default
        );

        Task UpdateAsync(
            long id,
            VectorTileModel model,
            string userId,
            CancellationToken token = default
        );

        Task DeleteAsync(
            long id,
            string userId,
            CancellationToken token = default
        );

        Task<TileContentModel> GetTileContentAsync(long id, int level, int row, int col);

        Task<DateTimeOffset?> GetTileModifiedTimeAsync(long id, int level, int row, int col);

        Task<JsonElement> GetStyleAsync(long id);

    }

}
