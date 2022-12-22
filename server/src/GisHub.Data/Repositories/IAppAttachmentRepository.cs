using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>附件表仓储接口</summary>
public partial interface IAppAttachmentRepository : IRepository<AppAttachmentModel, long> {

    Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
        AppAttachmentSearchModel model
    );

}
