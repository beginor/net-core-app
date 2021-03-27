using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>矢量切片包 仓储接口</summary>
    public partial interface IVectortileRepository : IRepository<VectortileModel, long> {

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<VectortileModel>> SearchAsync(
            VectortileSearchModel model
        );

        Task SaveAsync(
            VectortileModel model,
            string userId,
            CancellationToken token = default
        );

        Task UpdateAsync(
            long id,
            VectortileModel model,
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

    }

}
