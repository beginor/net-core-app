using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Slpk.Data {

    /// <summary>slpk 航拍模型 仓储接口</summary>
    public partial interface ISlpkRepository : IRepository<SlpkModel, long> {

        /// <summary>搜索 slpk 航拍模型 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<SlpkModel>> SearchAsync(
            SlpkSearchModel model
        );

        Task SaveAsync(SlpkModel model, string userId, CancellationToken token = default);

        Task UpdateAsync(long id, SlpkModel entity, string userId, CancellationToken token = default);

        Task DeleteAsync(long id, string userId, CancellationToken token = default);

    }

}
