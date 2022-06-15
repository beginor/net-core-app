using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Data.Repositories;
using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Slpk.Data; 

/// <summary>slpk 航拍模型仓储实现</summary>
public class SlpkRepository : HibernateRepository<SlpkEntity, SlpkModel, long>, ISlpkRepository {

    private IDistributedCache cache;
    private IAppStorageRepository storageRepository;
    private CommonOption commonOption;

    public SlpkRepository(
        ISession session,
        IMapper mapper,
        IDistributedCache cache,
        IAppStorageRepository storageRepository,
        CommonOption commonOption
    ) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            cache = null;
            storageRepository = null;
            commonOption = null;
        }
    }

    /// <summary>搜索 slpk 航拍模型 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<SlpkModel>> SearchAsync(
        SlpkSearchModel model
    ) {
        var query = Session.Query<SlpkEntity>();
        if (model.Keywords.IsNotNullOrEmpty()) {
            var keywords = model.Keywords;
            if (long.TryParse(keywords, out var id)) {
                query = query.Where(e => e.Id == id);
            }
            else {
                query = query.Where(
                    e => e.Name.Contains(keywords) || e.Description.Contains(keywords)
                );
            }
        }
        if (model.Category > 0) {
            query = query.Where(e => e.Category.Id == model.Category);
        }
        var total = await query.LongCountAsync();
        var data = await query.Select(e => new SlpkEntity {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Type = e.Type,
                Category = e.Category,
                Roles = e.Roles,
                Tags = e.Tags,
                Creator = e.Creator,
                CreatedAt = e.CreatedAt,
                Updater = e.Updater,
                UpdatedAt = e.UpdatedAt,
                Directory = e.Directory,
                Longitude = e.Longitude,
                Latitude = e.Latitude,
                Elevation = e.Elevation,
                IsDeleted = e.IsDeleted
            })
            .OrderByDescending(e => e.Id)
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<SlpkModel> {
            Total = total,
            Data = Mapper.Map<IList<SlpkModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task SaveAsync(SlpkModel model, AppUser user, CancellationToken token = default) {
        var entity = Mapper.Map<SlpkEntity>(model);
        entity.CreatedAt = DateTime.Now;
        entity.Creator = user;
        entity.UpdatedAt = DateTime.Now;
        entity.Updater = user;
        await Session.SaveAsync(entity, token);
        await Session.FlushAsync(token);
        await cache.RemoveAsync(entity.Id.ToString());
    }

    public async Task UpdateAsync(long id, SlpkModel model, AppUser user, CancellationToken token = default) {
        var entity = await Session.LoadAsync<SlpkEntity>(id, token);
        if (entity == null) {
            throw new InvalidOperationException(
                $"{typeof(SlpkModel)} with id {id} is null!"
            );
        }
        if (entity.Category.Id.ToString() != model.Category.Id) {
            entity.Category = Mapper.Map<Category>(model.Category);
        }
        Mapper.Map(model, entity);
        entity.UpdatedAt = DateTime.Now;
        entity.Updater = user;
        await Session.UpdateAsync(entity, token);
        await Session.FlushAsync(token);
        Mapper.Map(entity, model);
        await cache.RemoveAsync(id.ToString(), token);
    }

    public async Task DeleteAsync(long id, AppUser user, CancellationToken token = default) {
        var entity = await Session.GetAsync<SlpkEntity>(id, token);
        if (entity != null) {
            entity.UpdatedAt = DateTime.Now;
            entity.Updater = user;
            entity.IsDeleted = true;
            await Session.UpdateAsync(entity, token);
            await Session.FlushAsync(token);
        }
        await cache.RemoveAsync(id.ToString(), token);
    }

    public async Task<string> GetSlpkDirectoryAsync(long id) {
        var key = id.ToString();
        var cachedItem = await cache.GetAsync<SlpkCacheItem>(key);
        if (cachedItem != null) {
            return cachedItem.Directory;
        }
        var directory = await Session.Query<SlpkEntity>()
            .Where(e => e.Id == id)
            .Select(e => e.Directory)
            .FirstOrDefaultAsync();
        directory = await storageRepository.GetPhysicalPathAsync(directory);
        await cache.SetAsync(
            key,
            new SlpkCacheItem { Id = id, Directory = directory },
            commonOption.Cache.MemoryExpiration
        );
        return directory;
    }

}
