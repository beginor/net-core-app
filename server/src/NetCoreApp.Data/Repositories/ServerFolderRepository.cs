using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>服务器目录仓储实现</summary>
    public partial class ServerFolderRepository : HibernateRepository<ServerFolder, ServerFolderModel, long>, IServerFolderRepository {

        private readonly IDistributedCache cache;
        private readonly IWebHostEnvironment hostEnv;

        public ServerFolderRepository(ISession session, IMapper mapper, IDistributedCache cache, IWebHostEnvironment hostEnv) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.hostEnv = hostEnv ?? throw new ArgumentNullException(nameof(hostEnv));
        }

        /// <summary>搜索 服务器目录 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ServerFolderModel>> SearchAsync(
            ServerFolderSearchModel model
        ) {
            var query = Session.Query<ServerFolder>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<ServerFolderModel> {
                Total = total,
                Data = Mapper.Map<IList<ServerFolderModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task<ServerFolderBrowseModel> GetFolderContentAsync(string alias, string path, string searchPattern) {
            Argument.NotNullOrEmpty(path, nameof(path));
            var folderItem = await GetByAlias(alias);
            if (folderItem == null) {
                return null;
            }
            if (path.StartsWith(Path.PathSeparator)) {
                path = path.Substring(1);
            }
            var model = new ServerFolderBrowseModel {
                Path = $"{alias}:{path}"
            };
            var cachedItem = await GetCacheItemAsync(folderItem.Id);
            var serverPath = Path.Combine(cachedItem.RootFolder, path);
            var dirInfo = new DirectoryInfo(serverPath);
            if (!dirInfo.Exists) {
                return null;
            }
            model.Folders = dirInfo.EnumerateDirectories(searchPattern).Select(x => x.Name).ToArray();
            model.Files = dirInfo.EnumerateFiles(searchPattern).Select(x => x.Name).ToArray();
            return model;
        }

        public async Task<string> GetPhysicalPath(string alias, string path) {
            Argument.NotNullOrEmpty(alias, nameof(alias));
            Argument.NotNullOrEmpty(path, nameof(path));
            var folderItem = await GetByAlias(alias);
            if (folderItem == null) {
                return string.Empty;
            }
            if (path.StartsWith(Path.PathSeparator)) {
                path = path.Substring(1);
            }
            var cachedItem = await GetCacheItemAsync(folderItem.Id);
            var serverPath = Path.Combine(cachedItem.RootFolder, path);
            if (Directory.Exists(serverPath) || File.Exists(serverPath)) {
                return serverPath;
            }
            return string.Empty;
        }

        public async Task<Stream> GetFileContentAsync(string alias, string path) {
            Argument.NotNullOrEmpty(alias, nameof(alias));
            Argument.NotNullOrEmpty(path, nameof(path));
            var physicalPath = await GetPhysicalPath(alias, path);
            if (physicalPath.IsNullOrEmpty()) {
                return null;
            }
            return File.OpenRead(physicalPath);
        }

        private async Task<ServerFolder> GetByAlias(string alias) {
            var folder = await Session.Query<ServerFolder>()
                .FirstOrDefaultAsync(x => x.AliasName == alias);
            return folder;
        }

        private async Task<ServerFolderCacheItem> GetCacheItemAsync(long id) {
            var key = id.ToString();
            var item = await cache.GetAsync<ServerFolderCacheItem>(key);
            if (item == null) {
                var entity = await Session.LoadAsync<ServerFolder>(id);
                item = new ServerFolderCacheItem {
                    Id = entity.Id,
                    AliasName = entity.AliasName,
                    Readonly = entity.Readonly,
                    Roles = entity.Roles
                };
                var rootFolder = entity.RootFolder;
                if (rootFolder.StartsWith("!")) {
                    rootFolder = rootFolder.Substring(1);
                    if (rootFolder.StartsWith(Path.DirectorySeparatorChar)) {
                        rootFolder = rootFolder.Substring(1);
                    }
                }
                rootFolder = Path.Combine(hostEnv.WebRootPath, rootFolder);
                item.RootFolder = rootFolder;
                await cache.SetAsync(key, item);
            }
            return item;
        }

    }

}
