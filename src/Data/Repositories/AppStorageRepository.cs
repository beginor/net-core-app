using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    private CommonOption commonOption;
    private IFileProvider fileProvider;

    public AppStorageRepository(
        ISession session,
        IMapper mapper,
        IDistributedCache cache,
        CommonOption commonOption,
        IFileProvider fileProvider
    ) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
        this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
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
        var storage = await GetFromCacheByAliasAsync(model.Alias!);
        if (storage == null) {
            return null;
        }
        var rootFolder = storage.RootFolder;
        if (rootFolder.StartsWith("!")) {
            var fullPath = Path.Combine(rootFolder.Substring(1), model.Path);
            var dirContent = fileProvider.GetDirectoryContents(fullPath);
            if (!dirContent.Exists) {
                return null;
            }
            model.Folders = dirContent.Where(f => f.IsDirectory).Select(f => f.Name).ToArray();
            model.Files = dirContent.Where(f => !f.IsDirectory).Select(f => f.Name).ToArray();
        }
        else {
            var dirInfo = new DirectoryInfo(Path.Combine(storage.RootFolder, model.Path));
            if (!dirInfo.Exists) {
                return null;
            }
            model.Folders = dirInfo.EnumerateDirectories().Select(x => x.Name).ToArray();
            model.Files = dirInfo.EnumerateFiles(model.Filter).Select(x => x.Name).ToArray();
        }
        return model;
    }

    public async Task<Stream?> GetFileContentAsync(string alias, string path) {
        Argument.NotNullOrEmpty(alias, nameof(alias));
        Argument.NotNullOrEmpty(path, nameof(path));
        var storage = await GetFromCacheByAliasAsync(alias);
        if (storage == null) {
            return null;
        }
        var rootFolder = storage.RootFolder;
        if (rootFolder.StartsWith("!")) {
            var fullPath = Path.Combine(rootFolder.Substring(1), path);
            var fileInfo = fileProvider.GetFileInfo(fullPath);
            return fileInfo.Exists ? fileInfo.CreateReadStream() : null;
        }
        else {
            var fullPath = Path.Combine(storage.RootFolder, path.TrimStartDirectorySeparatorChar());
            var fileInfo = new FileInfo(fullPath);
            return fileInfo.Exists ? fileInfo.OpenRead() : null;
        }
    }

    public async Task<string> GetPhysicalPathAsync(string fullPath) {
        var idx = fullPath.IndexOf(':');
        if (idx == -1) {
            throw new ArgumentException($"{fullPath} does not contain an alias!");
        }
        var alias = fullPath.Substring(0, idx);
        var subPath = fullPath.Substring(idx + 1);
        var storage = await GetFromCacheByAliasAsync(alias);
        if (storage == null) {
            return string.Empty;
        }
        var rootFolder = storage.RootFolder;
        if (rootFolder.StartsWith("!")) {
            var path = Path.Combine(
                storage.RootFolder.Substring(1),
                subPath.TrimStartDirectorySeparatorChar()
            );
            if (fileProvider is CompositeFileProvider compositeFileProvider) {
                foreach (var provider in compositeFileProvider.FileProviders) {
                    if (provider is not PhysicalFileProvider physicalFileProvider) {
                        continue;
                    }
                    var fileInfo = physicalFileProvider.GetFileInfo(path);
                    if (fileInfo.Exists) {
                        return fileInfo.PhysicalPath!;
                    }
                }
            }
        }
        else {
            return Path.Combine(
                rootFolder,
                subPath.TrimStartDirectorySeparatorChar()
            );
        }
        return string.Empty;
    }

    private async Task<AppStorage?> GetFromCacheByAliasAsync(string alias) {
        var key = $"NetCoreApp_AppStorage_{alias}";
        var cachedStorage = await cache.GetAsync<AppStorage>(key);
        if (cachedStorage == null) {
            var storage = await Session.Query<AppStorage>().FirstOrDefaultAsync(
                x => x.AliasName == alias
            );
            if (storage == null) {
                return null;
            }
            cachedStorage = storage.Clone();
            await cache.SetAsync(
                key,
                cachedStorage,
                commonOption.Cache.MemoryExpiration
            );
        }
        return cachedStorage;
    }

}
