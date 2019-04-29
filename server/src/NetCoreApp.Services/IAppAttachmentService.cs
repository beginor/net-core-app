using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>附件表服务接口</summary>
    public partial interface IAppAttachmentService : IBaseService<AppAttachmentModel> {

        /// <summary>附件表搜索，返回分页结果。</summary>
        Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
            AppAttachmentSearchModel model
        );

    }

}
