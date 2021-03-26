using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.VectorTile.Models;

namespace Beginor.GisHub.VectorTile.Data {

    /// <summary>矢量切片包 仓储接口</summary>
    public partial interface IVectortileRepository : IRepository<VectortileModel, long> {

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<VectortileModel>> SearchAsync(
            VectortileSearchModel model
        );

    }

}
