using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.GisHub.TileMap.Models;
using NHibernate;
using NHibernate.Linq;

namespace Beginor.GisHub.TileMap.Data {

    /// <summary>切片地图仓储实现</summary>
    public partial class TileMapRepository : HibernateRepository<TileMapEntity, TileMapModel, long>, ITileMapRepository {

        private ConcurrentDictionary<long, TileMapCacheItem> cache;

        public TileMapRepository(
            ISession session,
            IMapper mapper,
            ConcurrentDictionary<long, TileMapCacheItem> cache
        ) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>搜索 切片地图 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<TileMapModel>> SearchAsync(
            TileMapSearchModel model
        ) {
            var query = Session.Query<TileMapEntity>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                var keywords = model.Keywords.Trim();
                query = query.Where(e => e.Name.Contains(keywords) || e.CacheDirectory.Contains(keywords));
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
            cache.TryRemove(entity.Id, out _);
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
            cache.TryRemove(id, out _);
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
            cache.TryRemove(id, out _);
        }

        public async Task<TileContentModel> GetTileContentAsync(long id, int level, int row, int col) {
            var tilePath = await GetTilePathAsync(id);
            if (tilePath.IsNullOrEmpty()) {
                return TileContentModel.Empty;
            }
            var tc = await ReadTileContentAsync(tilePath, level, row, col);
            if (cache.TryGetValue(id, out var ci)) {
                if (!string.IsNullOrEmpty(ci.ContentType)) {
                    tc.ContentType = ci.ContentType;
                }
            }
            return tc;
        }

        private async Task<TileMapEntity> GetTileMapByIdAsync(long id) {
            if (cache.TryGetValue(id, out var cacheItem)) {
                return new TileMapEntity {
                    Id = id,
                    Name = cacheItem.Name,
                    CacheDirectory = cacheItem.CacheDirectory,
                    MapTileInfoPath = cacheItem.MapTileInfoPath,
                    ContentType = cacheItem.ContentType
                };
            }
            var entity = await Session.Query<TileMapEntity>().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) {
                throw new TileNotFoundException($"Tilemap {id} doesn't exists in database.");
            }
            cache.TryAdd(id, new TileMapCacheItem {
                Name = entity.Name,
                CacheDirectory = entity.CacheDirectory,
                MapTileInfoPath = entity.MapTileInfoPath,
                ContentType = entity.ContentType,
                ModifiedTime = null
            });
            return entity;
        }

        public async Task<JsonElement> GetTileMapInfoAsync(long id) {
            var entity = await GetTileMapByIdAsync(id);
            var text = File.ReadAllText(entity.MapTileInfoPath)
                .Replace("#name#", entity.Name)
                .Replace("#description#", $"{entity.Name} Tile Server")
                .Replace("#copyright#", $"{entity.Name} Tile Server by GisHub");
            return JsonDocument.Parse(text).RootElement;
        }

        public async Task<DateTimeOffset?> GetTileModifiedTimeAsync(long id, int level, int row, int col) {
            if (cache.TryGetValue(id, out var cacheItem)) {
                if (cacheItem.ModifiedTime != null) {
                    return cacheItem.ModifiedTime.Value;
                }
            }
            var tilePath = await GetTilePathAsync(id);
            if (tilePath.IsNullOrEmpty()) {
                return null;
            }
            var rowGroup = GetGroupIndex(row);
            var colGroup = GetGroupIndex(col);
            var bundlePath = GetBundlePath(tilePath, level, rowGroup, colGroup);
            if (!File.Exists(bundlePath)) {
                return null;
            }
            var lastWriteTime = File.GetLastWriteTime(bundlePath);
            var offset = new DateTimeOffset(lastWriteTime);
            if (cache.TryGetValue(id, out var ci)) {
                ci.ModifiedTime = offset;
            }
            return offset;
        }

        private async Task<string> GetTilePathAsync(long id) {
            var entity = await GetTileMapByIdAsync(id);
            var tilePath = Path.Combine(entity.CacheDirectory, "_alllayers");
            if (Directory.Exists(tilePath)) {
                return tilePath;
            }
            return string.Empty;
        }

        private static async Task<TileContentModel> ReadTileContentAsync(string tilePath, int level, int row, int col) {
            var bundleContent = await ReadTileContentFromBundleAsync(tilePath, level, row, col);
            if (bundleContent.Content.Length > 0) {
                return bundleContent;
            }
            var fileContent = await ReadTileContentFromFileAsync(tilePath, level, row, col);
            if (fileContent.Content.Length > 0) {
                return bundleContent;
            }
            return TileContentModel.Empty;
        }

        private static async Task<TileContentModel> ReadTileContentFromFileAsync(string tilePath, int level, int row, int col) {
            // var tileFileName = string.Format("{0}\\L{1:D2}\\R{2:X8}\\C{3:X8}", tilePath, lev, r, c);
            var filePath = Path.Combine(tilePath, level.ToString("D2"), row.ToString("X8"), col.ToString("X8"));
            if (File.Exists(filePath + ".png")) {
                filePath = filePath + ".png";
            }
            else if (File.Exists(filePath + ".jpg")) {
                filePath = filePath + ".jpg";
            }
            else {
                return TileContentModel.Empty;
            }
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var length = (int)fs.Length;
            var buffer = new byte[length];
            await fs.ReadAsync(buffer, 0, length);
            fs.Close();
            return new TileContentModel {
                Content = buffer,
                ContentType = filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg"
            };
        }

        private static async Task<TileContentModel> ReadTileContentFromBundleAsync(string tilePath, int level, int row, int col) {
            var content = new TileContentModel();
            var rowGroup = GetGroupIndex(row);
            var colGroup = GetGroupIndex(col);
            // try get from bundle
            // string.Format("{0}\\L{1:D2}\\R{2:X4}C{3:X4}.{4}", tilePath, lev, rowGroup, colGroup, "bundle");
            var bundlePath = GetBundlePath(tilePath, level, rowGroup, colGroup);
            var index = 128 * (row - rowGroup) + (col - colGroup);
            if (string.IsNullOrEmpty(bundlePath) || !File.Exists(bundlePath)) {
                return TileContentModel.Empty;
            }
            using var fs = new FileStream(bundlePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            fs.Seek(64 + 8 * index, SeekOrigin.Begin);
            // 获取位置索引并计算切片位置偏移量
            var indexBytes = new byte[4];
            await fs.ReadAsync(indexBytes, 0, 4);
            var offset = (indexBytes[0] & 0xff)
                + (long)(indexBytes[1] & 0xff) * 256
                + (long)(indexBytes[2] & 0xff) * 65536
                + (long)(indexBytes[3] & 0xff) * 16777216;
            // 获取切片长度索引并计算切片长度
            var startOffset = offset - 4;
            fs.Seek(startOffset, SeekOrigin.Begin);
            var lengthBytes = new byte[4];
            await fs.ReadAsync(lengthBytes, 0, 4);
            var length = (lengthBytes[0] & 0xff)
                + (lengthBytes[1] & 0xff) * 256
                + (lengthBytes[2] & 0xff) * 65536
                + (lengthBytes[3] & 0xff) * 16777216;
            //根据切片位置和切片长度获取切片
            content.Content = new byte[length];
            await fs.ReadAsync(content.Content, 0, content.Content.Length);
            fs.Close();
            content.ContentType = "image/png";
            return content;
        }

        private static string GetBundlePath(string tileFolder, int level, int rowGroup, int colGroup) {
            var bundlePath = Path.Combine(tileFolder, $"L{level:D2}", $"R{rowGroup:X4}C{colGroup:X4}.bundle");
            if (File.Exists(bundlePath)) {
                return bundlePath;
            }
            bundlePath = Path.Combine(tileFolder, $"L{level:D2}", $"R{rowGroup:x4}C{colGroup:x4}.bundle");
            if (File.Exists(bundlePath)) {
                return bundlePath;
            }
            return string.Empty;
        }

        private static int GetGroupIndex(int x) {
            return 128 * (x / 128);
        }


    }

}
