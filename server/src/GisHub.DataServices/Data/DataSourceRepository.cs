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
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据源（数据表或视图）仓储实现</summary>
    public partial class DataSourceRepository : HibernateRepository<DataSource, DataSourceModel, long>, IDataSourceRepository {

        private ConcurrentDictionary<long, DataSourceCacheItem> cache;
        private IDataServiceFactory factory;

        public DataSourceRepository(
            ISession session,
            IMapper mapper,
            ConcurrentDictionary<long, DataSourceCacheItem> cache,
            IDataServiceFactory factory
        ) : base(session, mapper) {
            this.cache = cache;
            this.factory = factory;
        }

        protected override void Dispose(
            bool disposing
        ) {
            if (disposing) {
                cache = null;
                factory = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 数据源（数据表或视图） ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<DataSourceModel>> SearchAsync(
            DataSourceSearchModel model
        ) {
            var query = Session.Query<DataSource>();
            if (model.Keywords.IsNotNullOrEmpty()) {
                query = query.Where(
                    ds => ds.Name.Contains(model.Keywords) || ds.TableName.Contains(model.Keywords)
                );
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<DataSourceModel> {
                Total = total,
                Data = Mapper.Map<IList<DataSourceModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public override Task UpdateAsync(
            long id,
            DataSourceModel model,
            CancellationToken token = default
        ) {
            cache.TryRemove(id, out _);
            return base.UpdateAsync(id, model, token);
        }

        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<DataSource>(id);
            if (entity != null) {
                cache.TryRemove(id, out _);
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync(token);
            }
        }

        public async Task<DataSourceCacheItem> GetCacheItemByIdAsync(
            long id
        ) {
            if (cache.TryGetValue(id, out var item)) {
                return item;
            }
            var ds = await Session.Query<DataSource>()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
            if (ds == null) {
                return null;
            }
            item = new DataSourceCacheItem {
                DataSourceId = ds.Id,
                DataSourceName = ds.Name,
                DatabaseType = ds.Connection.DatabaseType,
                Schema = ds.Schema,
                TableName = ds.TableName,
                PrimaryKeyColumn = ds.PrimaryKeyColumn,
                DisplayColumn = ds.DisplayColumn,
                GeometryColumn = ds.GeometryColumn,
                PresetCriteria = ds.PresetCriteria,
                DefaultOrder = ds.DefaultOrder
            };
            var meta = factory.CreateMetadataProvider(item.DatabaseType);
            var model = Mapper.Map<ConnectionModel>(ds.Connection);
            item.ConnectionString = meta.BuildConnectionString(model);
            if (item.HasGeometryColumn) {
                var featureProvider = factory.CreateFeatureProvider(item.DatabaseType);
                item.Srid = await featureProvider.GetSridAsync(item);
                item.GeometryType = await featureProvider.GetGeometryTypeAsync(item);
            }
            cache.TryAdd(id, item);
            return item;
        }

    }

}
