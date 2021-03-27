using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.TileMap.Data;
using Beginor.GisHub.TileMap.Models;
using System.IO;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>矢量切片包仓储实现</summary>
    public partial class VectortileRepository : HibernateRepository<Vectortile, VectortileModel, long>, IVectortileRepository {

        private ConcurrentDictionary<long, TileMapCacheItem> cache;

        public VectortileRepository(
            ISession session,
            IMapper mapper,
            ConcurrentDictionary<long, TileMapCacheItem> cache
        ) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<VectortileModel>> SearchAsync(
            VectortileSearchModel model
        ) {
            var query = Session.Query<Vectortile>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<VectortileModel> {
                Total = total,
                Data = Mapper.Map<IList<VectortileModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task SaveAsync(
            VectortileModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = Mapper.Map<Vectortile>(model);
            entity.CreatedAt = DateTime.Now;
            entity.CreatorId = userId;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            cache.TryRemove(entity.Id, out _);
        }

        public async Task UpdateAsync(
            long id,
            VectortileModel model,
            string userId,
            CancellationToken token = default
        ) {
            var entity = await Session.LoadAsync<Vectortile>(id, token);
            if (entity == null) {
                throw new InvalidOperationException(
                    $"{typeof(VectortileModel)} with id {id} is null!"
                );
            }
            Mapper.Map(model, entity);
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
            await Session.UpdateAsync(entity, token);
            await Session.FlushAsync(token);
            Mapper.Map(entity, model);
            cache.TryRemove(id, out _);
        }


        public async Task DeleteAsync(long id, string userId, CancellationToken token = default) {
            var entity = Session.Get<Vectortile>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                entity.UpdaterId = userId;
                await Session.SaveAsync(entity, token);
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
            if (cache.TryGetValue(id, out var cacheItem)) {
                if (cacheItem.ModifiedTime != null) {
                    return cacheItem.ModifiedTime.Value;
                }
            }
            var tilemap = await GetVectorTileByIdAsync(id);
            if (tilemap.Directory.IsNullOrEmpty() || !Directory.Exists(tilemap.Directory)) {
                return null;
            }
            var offset = BundleHelper.GetTileModifiedTime(tilemap.Directory, level, row, col);
            if (offset != null && cache.TryGetValue(id, out var ci)) {
                ci.ModifiedTime = offset;
            }
            return offset;
        }

        private async Task<Vectortile> GetVectorTileByIdAsync(long id) {
            if (cache.TryGetValue(id, out var cacheItem)) {
                return new Vectortile {
                    Id = id,
                    Name = cacheItem.Name,
                    Directory = cacheItem.CacheDirectory
                };
            }
            var entity = await Session.Query<Vectortile>().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) {
                throw new TileNotFoundException($"Vectortile {id} doesn't exists in database.");
            }
            cache.TryAdd(id, new TileMapCacheItem {
                Name = entity.Name,
                CacheDirectory = entity.Directory,
                IsBundled = true,
                ModifiedTime = null
            });
            return entity;
        }


    }

}
