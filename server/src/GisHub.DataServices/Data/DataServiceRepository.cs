using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data;

/// <summary>数据源（数据表或视图）仓储实现</summary>
public partial class DataServiceRepository : HibernateRepository<DataService, DataServiceModel, long>, IDataServiceRepository {

    private IDistributedCache cache;
    private IDataServiceFactory factory;
    private CommonOption commonOption;

    public DataServiceRepository(
        ISession session,
        IMapper mapper,
        IDistributedCache cache,
        IDataServiceFactory factory,
        CommonOption commonOption
    ) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.factory = factory?? throw new ArgumentNullException(nameof(factory));
        this.commonOption = commonOption?? throw new ArgumentNullException(nameof(commonOption));
    }

    protected override void Dispose(
        bool disposing
    ) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 数据源（数据表或视图） ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<DataServiceModel>> SearchAsync(
        DataServiceSearchModel model
    ) {
        var query = Session.Query<DataService>();
        if (model.Keywords.IsNotNullOrEmpty()) {
            var keywords = model.Keywords.Trim();
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
        var data = await query.Select(x => new DataService {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Schema = x.Schema,
                TableName = x.TableName,
                PrimaryKeyColumn = x.PrimaryKeyColumn,
                DisplayColumn = x.DisplayColumn,
                GeometryColumn = x.GeometryColumn,
                SupportMvt = x.SupportMvt,
                MvtMinZoom = x.MvtMinZoom,
                MvtMaxZoom = x.MvtMaxZoom,
                MvtCacheDuration = x.MvtCacheDuration
            })
            .OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<DataServiceModel> {
            Total = total,
            Data = Mapper.Map<IList<DataServiceModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task UpdateAsync(
        long id,
        DataServiceModel model,
        AppUser user,
        CancellationToken token = default
    ) {
        var entity = await Session.LoadAsync<DataService>(id, token);
        if (entity.DataSource.Id.ToString() != model.DataSource.Id) {
            entity.DataSource = Mapper.Map<DataSource>(model.DataSource);
        }
        if (entity.Category.Id.ToString() != model.Category.Id) {
            entity.Category = Mapper.Map<Category>(model.Category);
        }
        Mapper.Map(model, entity);
        entity.Updater = user;
        entity.UpdatedAt = DateTime.Now;
        await Session.UpdateAsync(entity, token);
        await Session.FlushAsync(token);
        Session.Clear();
        await cache.RemoveAsync(id.ToString(), token);
    }

    public async Task DeleteAsync(long id, AppUser user, CancellationToken token = default) {
        var entity = Session.Get<DataService>(id);
        if (entity != null) {
            await cache.RemoveAsync(id.ToString(), token);
            entity.IsDeleted = true;
            entity.Updater = user;
            entity.UpdatedAt = DateTime.Now;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
        }
    }

    public async Task<DataServiceCacheItem?> GetCacheItemByIdAsync(long id) {
        var key = id.ToString();
        var item = await cache.GetAsync<DataServiceCacheItem>(key);
        if (item != null) {
            return item;
        }
        var ds = await Session.Query<DataService>()
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
        if (ds == null) {
            return null;
        }
        item = new DataServiceCacheItem {
            DataServiceId = ds.Id,
            DataServiceName = ds.Name,
            DataServiceDescription = ds.Description,
            DatabaseType = ds.DataSource.DatabaseType,
            Schema = ds.Schema,
            TableName = ds.TableName,
            PrimaryKeyColumn = ds.PrimaryKeyColumn,
            DisplayColumn = ds.DisplayColumn,
            GeometryColumn = ds.GeometryColumn,
            PresetCriteria = ds.PresetCriteria,
            DefaultOrder = ds.DefaultOrder,
            Fields = ds.Fields,
            SupportMvt = ds.SupportMvt.GetValueOrDefault(false),
            MvtMinZoom = ds.MvtMinZoom.GetValueOrDefault(0),
            MvtMaxZoom = ds.MvtMaxZoom.GetValueOrDefault(0),
            MvtCacheDuration = ds.MvtCacheDuration.GetValueOrDefault(0)
        };
        var meta = factory.CreateMetadataProvider(item.DatabaseType);
        if (meta == null) {
            return null;
        }
        var model = Mapper.Map<DataSourceModel>(ds.DataSource);
        item.ConnectionString = meta.BuildConnectionString(model);
        if (item.HasGeometryColumn) {
            var featureProvider = factory.CreateFeatureProvider(item.DatabaseType);
            if (featureProvider == null) {
                return null;
            }
            item.Srid = await featureProvider.GetSridAsync(item);
            item.GeometryType = await featureProvider.GetGeometryTypeAsync(item);
        }
        await cache.SetAsync(key, item, commonOption.Cache.MemoryExpiration);
        return item;
    }

    public async Task SaveAsync(DataServiceModel model, AppUser user, CancellationToken token = default) {
        var entity = Mapper.Map<DataService>(model);
        entity.Creator = user;
        entity.CreatedAt = DateTime.Now;
        entity.Updater = user;
        entity.UpdatedAt = DateTime.Now;
        await Session.SaveAsync(entity, token);
        await Session.FlushAsync(token);
        Session.Clear();
        Mapper.Map(entity, model);
    }

}
