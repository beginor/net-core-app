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
    private CommonOption commonOption;
    private IWebHostEnvironment webHostEnv;
    private IFileProvider fileProvider;

    public AppStorageRepository(
        ISession session,
        IMapper mapper,
        IDistributedCache cache,
        CommonOption commonOption,
        IWebHostEnvironment webHostEnv,
        IFileProvider fileProvider
    ) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
        this.webHostEnv = webHostEnv ?? throw new ArgumentNullException(nameof(webHostEnv));
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
        var cacheItem = await GetCacheItemByAliasAsync(model.Alias!);
        if (cacheItem == null) {
            return null;
        }
        var rootFolder = cacheItem.RootFolder;
        var fullPath = rootFolder.StartsWith("!")
            ? Path.Combine(webHostEnv.WebRootPath + rootFolder.Substring(1), model.Path)
            : Path.Combine(Path.Combine(cacheItem.RootFolder, model.Path));
        var dirInfo = new DirectoryInfo(fullPath);
        if (!dirInfo.Exists) {
            return null;
        }
        model.Folders = dirInfo.EnumerateDirectories().Select(x => x.Name).ToArray();
        var files = new List<string>();

        foreach (var filter in model.Filter.Split(";")) {
            var fileNames = dirInfo.EnumerateFiles(filter).Select(file => file.Name);
            files.AddRange(fileNames);
        }
        model.Files = files.ToArray();
        return model;
    }

    public async Task<Stream?> GetFileContentAsync(string alias, string path) {
        Argument.NotNullOrEmpty(alias, nameof(alias));
        Argument.NotNullOrEmpty(path, nameof(path));
        var cacheItem = await GetCacheItemByAliasAsync(alias);
        if (cacheItem == null) {
            return null;
        }
        var rootFolder = cacheItem.RootFolder;
        if (rootFolder.StartsWith("!")) {
            var fullPath = Path.Combine(rootFolder.Substring(1), path);
            var fileInfo = fileProvider.GetFileInfo(fullPath);
            return fileInfo.Exists ? fileInfo.CreateReadStream() : null;
        }
        else {
            var fullPath = Path.Combine(cacheItem.RootFolder, path.TrimStartDirectorySeparatorChar());
            var fileInfo = new FileInfo(fullPath);
            return fileInfo.Exists ? fileInfo.OpenRead() : null;
        }
    }

    private async Task<AppStorageCacheItem?> GetCacheItemByAliasAsync(string alias) {
        var key = $"NetCoreApp_AppStorage_{alias}";
        var cacheItem = await cache.GetAsync<AppStorageCacheItem>(key);
        if (cacheItem == null) {
            var entity = await Session.Query<AppStorage>().FirstOrDefaultAsync(
                x => x.AliasName == alias
            );
            if (entity == null) {
                return null;
            }
            cacheItem = new AppStorageCacheItem {
                Id = entity.Id,
                AliasName = entity.AliasName,
                Readonly = entity.Readonly,
                Roles = entity.Roles,
                RootFolder = entity.RootFolder,
            };
            await cache.SetAsync(
                key,
                cacheItem,
                commonOption.Cache.MemoryExpiration
            );
        }
        return cacheItem;
    }

}
