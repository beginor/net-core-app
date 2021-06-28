using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>服务器目录 仓储接口</summary>
    public partial interface IServerFolderRepository : IRepository<ServerFolderModel, long> {

        /// <summary>搜索 服务器目录 ，返回分页结果。</summary>
        Task<PaginatedResponseModel<ServerFolderModel>> SearchAsync(
            ServerFolderSearchModel model
        );

        Task<ServerFolderBrowseModel> GetFolderContentAsync(ServerFolderBrowseModel model);

        Task<string> GetPhysicalPath(string alias, string path);

        Task<Stream> GetFileContentAsync(string alias, string path);

    }

}
