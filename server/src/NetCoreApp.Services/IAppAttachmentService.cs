using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>附件服务接口</summary>
    public interface IAppAttachmentService : IBaseService<AppAttachmentModel> {

        /// <summary>附件搜索，返回分页结果</summary>
        Task<PaginatedResponseModel<AppAttachmentModel>> Search(
            AppAttachmentSearchModel model
        );

        /// <summary>获取指定用户的附件</summary>
        Task<IList<AppAttachmentModel>> GetByUser(string userId);

    }

}
