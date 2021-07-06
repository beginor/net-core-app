using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.TileMap.Models;
using NHibernate;
using NHibernate.Linq;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>切片地图仓储实现</summary>
    public partial class TileMapRepository : HibernateRepository<TileMapEntity, TileMapModel, long>, ITileMapRepository {

        private IDistributedCache cache;
        private IServerFolderRepository serverFolderRepository;

        public TileMapRepository(
            ISession session,
            IMapper mapper,
            IDistributedCache cache,
            IServerFolderRepository serverFolderRepository
        ) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.serverFolderRepository = serverFolderRepository ?? throw new ArgumentNullException(nameof(serverFolderRepository));
        }

        /// <summary>搜索 切片地图 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<TileMapModel>> SearchAsync(
            TileMapSearchModel model
        ) {
            var query = Session.Query<TileMapEntity>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                var keywords = model.Keywords.Trim();
                if (long.TryParse(keywords, out var id)) {
                    query = query.Where(e => e.Id == id);
                }
                else {
                    query = query.Where(e => e.Name.Contains(keywords) || e.CacheDirectory.Contains(keywords));
                }
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.UpdatedAt)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<TileMapModel> {
                Total = total,
                Data = Mapper.Map<IList<TileMapModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task SaveAsync(
            TileMapModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = Mapper.Map<TileMapEntity>(model);
            entity.CreatedAt = DateTime.Now;
            entity.CreatorId = userId;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            await cache.RemoveAsync(entity.Id.ToString(), token);
        }

        public async Task UpdateAsync(
            long id,
            TileMapModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = await Session.LoadAsync<TileMapEntity>(id, token);
            if (entity == null) {
                throw new InvalidOperationException(
                    $"{typeof(TileMapModel)} with id {id} is null!"
                );
            }
            Mapper.Map(model, entity);
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            await Session.UpdateAsync(entity, token);
            await Session.FlushAsync(token);
            Mapper.Map(entity, model);
            await cache.RemoveAsync(id.ToString(), token);
        }

        public async Task DeleteAsync(
            long id,
            string userId,
            CancellationToken token = default
        ) {
            var entity = await Session.GetAsync<TileMapEntity>(id, token);
            if (entity != null) {
                entity.UpdatedAt = DateTime.Now;
                entity.UpdaterId = userId;
                entity.IsDeleted = true;
                await Session.UpdateAsync(entity, token);
                await Session.FlushAsync(token);
            }
            await cache.RemoveAsync(id.ToString(), token);
        }

        public async Task<TileContentModel> GetTileContentAsync(long id, int level, int row, int col) {
            var tilemap = await GetTileMapByIdAsync(id);
            if (!Directory.Exists(tilemap.CacheDirectory)) {
                return TileContentModel.Empty;
            }
            if (tilemap.IsBundled) {
                var bundleTile = await BundleHelper.ReadTileContentAsync(tilemap.CacheDirectory, level, row, col);
                bundleTile.ContentType = tilemap.ContentType;
                return bundleTile;
            }
            var fileTile = await FileHelper.ReadTileContentAsync(tilemap.CacheDirectory, level, row, col);
            fileTile.ContentType = tilemap.ContentType;
            return fileTile;
        }

        private async Task<TileMapEntity> GetTileMapByIdAsync(long id) {
            var key = id.ToString();
            var cachedItem = await cache.GetAsync<TileMapCacheItem>(key);
            if (cachedItem != null) {
                return new TileMapEntity {
                    Id = id,
                    Name = cachedItem.Name,
                    CacheDirectory = cachedItem.CacheDirectory,
                    MapTileInfoPath = cachedItem.MapTileInfoPath,
                    ContentType = cachedItem.ContentType,
                    IsBundled = cachedItem.IsBundled
                };
            }
            var entity = await Session.Query<TileMapEntity>().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) {
                throw new TileNotFoundException($"Tilemap {id} doesn't exists in database.");
            }
            entity.CacheDirectory = await serverFolderRepository.GetPhysicalPathAsync(entity.CacheDirectory);
            entity.MapTileInfoPath = await serverFolderRepository.GetPhysicalPathAsync(entity.MapTileInfoPath);
            await cache.SetAsync(key, new TileMapCacheItem {
                Name = entity.Name,
                CacheDirectory = entity.CacheDirectory,
                MapTileInfoPath = entity.MapTileInfoPath,
                ContentType = entity.ContentType,
                IsBundled = entity.IsBundled,
                ModifiedTime = null,
                MinLevel = entity.MinLevel,
                MaxLevel = entity.MaxLevel
            });
            return entity;
        }

        public async Task<JsonElement> GetTileMapInfoAsync(long id) {
            var entity = await GetTileMapByIdAsync(id);
            var text = File.ReadAllText(entity.MapTileInfoPath)
                .Replace("#name#", entity.Name)
                .Replace("#description#", $"{entity.Name} Tile Server")
                .Replace("#copyright#", $"{entity.Name} Tile Server by GISHub");
            return JsonDocument.Parse(text).RootElement;
        }

        public async Task<DateTimeOffset?> GetTileModifiedTimeAsync(long id, int level, int row, int col) {
            var key = id.ToString();
            var cacheItem = await cache.GetAsync<TileMapCacheItem>(key);
            if (cacheItem != null) {
                if (cacheItem.ModifiedTime != null) {
                    return cacheItem.ModifiedTime.Value;
                }
            }
            var tilemap = await GetTileMapByIdAsync(id);
            if (tilemap.CacheDirectory.IsNullOrEmpty() || !Directory.Exists(tilemap.CacheDirectory)) {
                return null;
            }
            var offset = BundleHelper.GetTileModifiedTime(tilemap.CacheDirectory, level, row, col);
            if (offset != null ) {
                var ci = await cache.GetAsync<TileMapCacheItem>(key);
                if (ci != null) {
                    ci.ModifiedTime = offset;
                    await cache.SetAsync(key, ci);
                }
            }
            return offset;
        }

    }

}
