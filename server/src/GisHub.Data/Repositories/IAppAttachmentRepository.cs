using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>附件表仓储接口</summary>
public partial interface IAppAttachmentRepository : IRepository<AppAttachmentModel, long> {

    Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
        AppAttachmentSearchModel model
    );

    Task SaveContentAsync(long id, byte[] content, CancellationToken token = default);

    Task<byte[]> GetContentAsync(long id, CancellationToken token = default);

    Task SaveAsync(AppAttachmentModel model, byte[] content, AppUser user, CancellationToken token = default);

}
