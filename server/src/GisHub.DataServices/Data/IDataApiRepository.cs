using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据API 仓储接口</summary>
    public partial interface IDataApiRepository : IRepository<DataApiModel, long> {

        /// <summary>搜索 数据API ，返回分页结果。</summary>
        Task<PaginatedResponseModel<DataApiModel>> SearchAsync(
            DataApiSearchModel model
        );

        Task SaveAsync(DataApiModel model, AppUser user, CancellationToken token = default);

        Task DeleteAsync(long id, AppUser user, CancellationToken token = default);

        Task UpdateAsync(long id, DataApiModel model, AppUser user, CancellationToken token = default);

    }

}
