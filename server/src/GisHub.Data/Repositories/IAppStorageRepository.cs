using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>应用存储 仓储接口</summary>
public partial interface IAppStorageRepository : IRepository<AppStorageModel, long> {

    /// <summary>搜索 应用存储 ，返回分页结果。</summary>
    Task<PaginatedResponseModel<AppStorageModel>> SearchAsync(
        AppStorageSearchModel model
    );

    Task<AppStorageBrowseModel?> GetFolderContentAsync(AppStorageBrowseModel model);

    Task<string> GetPhysicalPathAsync(string aliasedPath);

    Task<Stream?> GetFileContentAsync(string alias, string path);

}
