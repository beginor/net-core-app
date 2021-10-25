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
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Common;
using Beginor.GisHub.TileMap.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>矢量切片包仓储实现</summary>
    public partial class VectorTileRepository : HibernateRepository<VectorTileEntity, VectorTileModel, long>, IVectorTileRepository {

        private IDistributedCache cache;
        private IAppJsonDataRepository jsonRepository;
        private IAppStorageRepository storageRepository;
        private CommonOption commonOption;

        public VectorTileRepository(
            ISession session,
            IMapper mapper,
            IDistributedCache cache,
            IAppJsonDataRepository jsonRepository,
            IAppStorageRepository storageRepository,
            CommonOption commonOption
        ) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.jsonRepository = jsonRepository ?? throw new ArgumentNullException(nameof(jsonRepository));
            this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
            this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                cache = null;
                storageRepository = null;
                jsonRepository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<VectorTileModel>> SearchAsync(
            VectorTileSearchModel model
        ) {
            var query = Session.Query<VectorTileEntity>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                var keywords = model.Keywords.Trim();
                if (keywords.IsNotNullOrEmpty()) {
                    if (long.TryParse(keywords, out var id)) {
                        query = query.Where(e => e.Id == id);
                    }
                    else {
                        query = query.Where(e => e.Name.Contains(keywords));
                    }
                }
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<VectorTileModel> {
                Total = total,
                Data = Mapper.Map<IList<VectorTileModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task SaveAsync(
            VectorTileModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = Mapper.Map<VectorTileEntity>(model);
            entity.CreatedAt = DateTime.Now;
            entity.CreatorId = userId;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            using var trans = Session.BeginTransaction();
            try {
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync(token);
                var styleContent = model.StyleContent;
                if (styleContent.IsNotNullOrEmpty()) {
                    styleContent = styleContent.Replace("{id}", entity.Id.ToString());
                }
                await TrySaveStyleContent(entity.Id, styleContent);
                await trans.CommitAsync();
                await cache.RemoveAsync(entity.Id.ToString(), token);
            }
            catch (Exception) {
                await trans.RollbackAsync();
                throw;
            }
        }

        private async Task TrySaveStyleContent(long id, string styleContent) {
            if (styleContent.IsNotNullOrEmpty()) {
                var jsonDoc = JsonDocument.Parse(styleContent);
                await jsonRepository.SaveValueAsync(id, jsonDoc.RootElement);
            }
        }

        public async Task UpdateAsync(
            long id,
            VectorTileModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = await Session.LoadAsync<VectorTileEntity>(id, token);
            if (entity == null) {
                throw new InvalidOperationException(
                    $"{typeof(VectorTileModel)} with id {id} is null!"
                );
            }
            Mapper.Map(model, entity);
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            using var trans = Session.BeginTransaction();
            try {
                await Session.UpdateAsync(entity, token);
                await Session.FlushAsync(token);
                await TrySaveStyleContent(id, model.StyleContent);
                await trans.CommitAsync();
                Mapper.Map(entity, model);
                await cache.RemoveAsync(id.ToString(), token);
            }
            catch (Exception) {
                await trans.RollbackAsync();
                throw;
            }
        }


        public async Task DeleteAsync(long id, string userId, CancellationToken token = default) {
            var entity = Session.Get<VectorTileEntity>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                entity.UpdaterId = userId;
                await Session.SaveAsync(entity, token);
                await jsonRepository.DeleteAsync(id);
                await Session.FlushAsync();
            }
        }

        public async Task<TileContentModel> GetTileContentAsync(long id, int level, int row, int col) {
            var entity = await GetVectorTileByIdAsync(id);
            var tileContent = await BundleHelper.ReadTileContentAsync(entity.Directory, level, row, col);
            tileContent.ContentType = "application/octet-stream";
            return tileContent;
        }

        public async Task<DateTimeOffset?> GetTileModifiedTimeAsync(long id, int level, int row, int col) {
            var key = id.ToString();
            var cacheItem = await cache.GetAsync<TileMapCacheItem>(key);
            if (cacheItem != null && cacheItem.ModifiedTime != null) {
                return cacheItem.ModifiedTime.Value;
            }
            var tilemap = await GetVectorTileByIdAsync(id);
            if (tilemap.Directory.IsNullOrEmpty() || !Directory.Exists(tilemap.Directory)) {
                return null;
            }
            var offset = BundleHelper.GetTileModifiedTime(tilemap.Directory, level, row, col);
            if (offset != null) {
                var ci = await cache.GetAsync<TileMapCacheItem>(key);
                if (ci != null) {
                    ci.ModifiedTime = offset;
                    await cache.SetAsync(key, ci, commonOption.Cache.MemoryExpiration);
                }
            }
            return offset;
        }

        private async Task<VectorTileEntity> GetVectorTileByIdAsync(long id) {
            var key = id.ToString();
            var cacheItem = await cache.GetAsync<TileMapCacheItem>(key);
            if (cacheItem != null) {
                return cacheItem.ToVectorTileEntity();
            }
            var entity = await Session.GetAsync<VectorTileEntity>(id);
            if (entity == null) {
                throw new TileNotFoundException($"Vectortile {id} doesn't exists in database.");
            }
            cacheItem = entity.ToCache();
            cacheItem.CacheDirectory = await storageRepository.GetPhysicalPathAsync(entity.Directory);
            await cache.SetAsync(key, cacheItem, commonOption.Cache.MemoryExpiration);
            return cacheItem.ToVectorTileEntity();
        }

        public Task<JsonElement> GetStyleAsync(long id) {
            return jsonRepository.GetValueByIdAsync(id);
        }

    }

}
