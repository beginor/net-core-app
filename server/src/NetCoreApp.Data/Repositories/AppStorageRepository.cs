using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>应用存储仓储实现</summary>
public partial class AppStorageRepository : HibernateRepository<AppStorage, AppStorageModel, long>, IAppStorageRepository {

    private IDistributedCache cache;
    private IFileProvider fileProvider;
    private CommonOption commonOption;

    public AppStorageRepository(
        ISession session,
        IMapper mapper,
        IDistributedCache cache,
        IFileProvider fileProvider,
        CommonOption commonOption
    ) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 应用存储 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppStorageModel>> SearchAsync(
        AppStorageSearchModel model
    ) {
        var query = Session.Query<AppStorage>();
        var keywords = model.Keywords;
        if (keywords.IsNotNullOrEmpty()) {
            query = query.Where(
                f => f.AliasName!.Contains(keywords!) || f.RootFolder!.Contains(keywords!)
            );
        }
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppStorageModel> {
            Total = total,
            Data = Mapper.Map<IList<AppStorageModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task<AppStorageBrowseModel?> GetFolderContentAsync(AppStorageBrowseModel model) {
        var folderItem = await GetByAlias(model.Alias!);
        if (folderItem == null) {
            return null;
        }
        if (model.Path!.StartsWith(Path.DirectorySeparatorChar)) {
            model.Path = model.Path.Substring(1);
        }
        var item = await GetByAlias(model.Alias);
        var path = model.Path;
        if (path.StartsWith(Path.PathSeparator)) {
            path = path.Substring(1);
        }
        path = Path.Combine(item.RootFolder, path);
        var dirInfo = fileProvider.GetDirectoryContents(path);
        if (!dirInfo.Exists) {
            return null;
        }
        model.Folders = dirInfo.Where(f => f.IsDirectory).Select(f => f.Name).ToArray();
        model.Files = dirInfo.Where(f => !f.IsDirectory).Select(x => x.Name).ToArray();
        return model;
    }

    public async Task<string> GetPhysicalPathAsync(string aliasedPath) {
        Argument.NotNullOrEmpty(aliasedPath, nameof(aliasedPath));
        var idx = aliasedPath.IndexOf(':');
        if (idx < 0) {
            throw new InvalidOperationException($"Invalid path {aliasedPath}");
        }
        var alias = aliasedPath.Substring(0, idx);
        var path = aliasedPath.Substring(idx + 1);
        var folderItem = await GetByAlias(alias);
        if (folderItem == null) {
            return string.Empty;
        }
        var item = await GetByAlias(alias);
        var fileInfo = fileProvider.GetFileInfo(path);
        var result = string.Empty;
        if (fileInfo.Exists) {
            result = fileInfo.PhysicalPath;
        }
        return result!;
    }

    public async Task<Stream?> GetFileContentAsync(string alias, string path) {
        Argument.NotNullOrEmpty(alias, nameof(alias));
        Argument.NotNullOrEmpty(path, nameof(path));
        var item = await GetByAlias(alias);
        var fileInfo = fileProvider.GetFileInfo(path);
        if (!fileInfo.Exists) {
            return null;
        }
        return fileInfo.CreateReadStream();
    }

    private async Task<AppStorage> GetByAlias(string alias) {
        var folder = await Session.Query<AppStorage>()
            .FirstOrDefaultAsync(x => x.AliasName == alias);
        return folder;
    }

}
