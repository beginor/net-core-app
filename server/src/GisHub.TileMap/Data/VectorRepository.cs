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
    public partial class VectorTileRepository : HibernateRepository<VectorTileEntity, VectorTileModel, long>, IVectorTileRepository {

        private ConcurrentDictionary<long, TileMapCacheItem> cache;

        public VectorTileRepository(
            ISession session,
            IMapper mapper,
            ConcurrentDictionary<long, TileMapCacheItem> cache
        ) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>搜索 矢量切片包 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<VectorTileModel>> SearchAsync(
            VectortileSearchModel model
        ) {
            var query = Session.Query<VectorTileEntity>();
            // todo: 添加自定义查询；
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
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            cache.TryRemove(entity.Id, out _);
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
            await Session.UpdateAsync(entity, token);
            await Session.FlushAsync(token);
            Mapper.Map(entity, model);
            cache.TryRemove(id, out _);
        }


        public async Task DeleteAsync(long id, string userId, CancellationToken token = default) {
            var entity = Session.Get<VectorTileEntity>(id);
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

        private async Task<VectorTileEntity> GetVectorTileByIdAsync(long id) {
            if (cache.TryGetValue(id, out var cacheItem)) {
                return new VectorTileEntity {
                    Id = id,
                    Name = cacheItem.Name,
                    Directory = cacheItem.CacheDirectory
                };
            }
            var entity = await Session.Query<VectorTileEntity>().FirstOrDefaultAsync(e => e.Id == id);
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
