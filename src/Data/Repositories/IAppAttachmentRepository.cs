using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>附件表仓储接口</summary>
public partial interface IAppAttachmentRepository : IRepository<AppAttachmentModel, long> {

    Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
        AppAttachmentSearchModel model
    );

    Task<byte[]> GetThumbnailAsync(long id, CancellationToken token = default);

    Task SaveAsync(AppAttachmentModel model, FileInfo file, ClaimsPrincipal user, CancellationToken token = default);

    string GetAttachmentStorageDirectory();

    string GetAttachmentTempDirectory(string userId);

    Task<int> DeleteByBusinessIdAsync(long businessId, CancellationToken token = default);

}
