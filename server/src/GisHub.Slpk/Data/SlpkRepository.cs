using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.GisHub.Common;
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
        var sql = new StringBuilder();
        sql.AppendLine("from public.slpks ");
        sql.AppendLine("where is_deleted = false ");
        if (model.Keywords.IsNotNullOrEmpty()) {
            sql.AppendLine(" and (directory like '%' || @Keywords || '%' or @Keywords = any(tags)) ");
        }
        var total = await Session.Connection.ExecuteScalarAsync<long>(
            "select count(*) " + sql.ToString(),
            model
        );
        sql.AppendLine(" order by updated_at desc ");
        sql.AppendLine(" limit @Take offset @Skip ");
        var data = await Session.Connection.QueryAsync<SlpkEntity>(
            "select * " + sql.ToString(),
            model
        );
        return new PaginatedResponseModel<SlpkModel> {
            Total = total,
            Data = Mapper.Map<IList<SlpkModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task SaveAsync(SlpkModel model, string userId, CancellationToken token = default) {
        var entity = Mapper.Map<SlpkEntity>(model);
        entity.CreatedAt = DateTime.Now;
        entity.CreatorId = userId;
        entity.UpdatedAt = DateTime.Now;
        entity.UpdaterId = userId;
        await Session.SaveAsync(entity, token);
        await Session.FlushAsync(token);
        await cache.RemoveAsync(entity.Id.ToString());
    }

    public async Task UpdateAsync(long id, SlpkModel model, string userId, CancellationToken token = default) {
        var entity = await Session.LoadAsync<SlpkEntity>(id, token);
        if (entity == null) {
            throw new InvalidOperationException(
                $"{typeof(SlpkModel)} with id {id} is null!"
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

    public async Task DeleteAsync(long id, string userId, CancellationToken token = default) {
        var entity = await Session.GetAsync<SlpkEntity>(id, token);
        if (entity != null) {
            entity.UpdatedAt = DateTime.Now;
            entity.UpdaterId = userId;
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