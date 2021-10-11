using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据源（数据表或视图）仓储实现</summary>
    public partial class DataServiceRepository : HibernateRepository<DataService, DataServiceModel, long>, IDataServiceRepository {

        private IDistributedCache cache;
        private IDataServiceFactory factory;

        public DataServiceRepository(
            ISession session,
            IMapper mapper,
            IDistributedCache cache,
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
        public async Task<PaginatedResponseModel<DataServiceModel>> SearchAsync(
            DataServiceSearchModel model,
            string[] roles
        ) {
            var query = Session.Query<DataService>();
            var totalSql = new StringBuilder(" select count(ds.*) ");
            var dataSql = new StringBuilder(" select ds.id, ds.name, ds.description, ds.schema, ds.table_name, ds.primary_key_column, ds.display_column, ds.geometry_column, ds.support_mvt ");
            var body = new StringBuilder();
            body.AppendLine(" from public.data_services as ds ");
            // body.AppendLine(" inner join connections c on c.id = ds.connection_id and c.is_deleted = false ");
            body.AppendLine(" where (ds.is_deleted = false) and (ds.roles && @roles::character varying[]) ");
            //
            long id = 0;
            if (model.Keywords.IsNotNullOrEmpty()) {
                body.AppendLine(
                    long.TryParse(model.Keywords, out id)
                        ? " and (ds.id = @id) "
                        : " and (ds.name like '%' || @keywords || '%' or ds.table_name like '%' || @keywords || '%') "
                );
            }
            var param = new {
                roles,
                id,
                keywords = model.Keywords,
                take = model.Take,
                skip = model.Skip
            };
            totalSql.Append(body.ToString());
            var total = await Session.Connection.ExecuteScalarAsync<long>(totalSql.ToString(), param);
            dataSql.Append(body.ToString());
            dataSql.AppendLine(" order by ds.id desc ");
            dataSql.AppendLine(" limit @take offset @skip ");
            // Dapper.SqlMapper.AddTypeHandler(new JsonTypedHandler<DataSourceField[]>());
            var data = await Session.Connection.QueryAsync<DataService>(
                sql: dataSql.ToString(),
                param: param
            );
            return new PaginatedResponseModel<DataServiceModel> {
                Total = total,
                Data = Mapper.Map<IList<DataServiceModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public override async Task UpdateAsync(
            long id,
            DataServiceModel model,
            CancellationToken token = default
        ) {
            await cache.RemoveAsync(id.ToString(), token);
            await base.UpdateAsync(id, model, token);
        }

        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<DataService>(id);
            if (entity != null) {
                await cache.RemoveAsync(id.ToString(), token);
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync(token);
            }
        }

        public async Task<DataServiceCacheItem> GetCacheItemByIdAsync(
            long id
        ) {
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
                DatabaseType = ds.DataSource.DatabaseType,
                Schema = ds.Schema,
                TableName = ds.TableName,
                PrimaryKeyColumn = ds.PrimaryKeyColumn,
                DisplayColumn = ds.DisplayColumn,
                GeometryColumn = ds.GeometryColumn,
                PresetCriteria = ds.PresetCriteria,
                DefaultOrder = ds.DefaultOrder,
                Roles = ds.Roles,
                Fields = ds.Fields,
                SupportMvt = ds.SupportMvt,
                MvtMinZoom = ds.MvtMinZoom,
                MvtMaxZoom = ds.MvtMaxZoom,
                MvtCacheDuration = ds.MvtCacheDuration
            };
            var meta = factory.CreateMetadataProvider(item.DatabaseType);
            var model = Mapper.Map<DataSourceModel>(ds.DataSource);
            item.ConnectionString = meta.BuildConnectionString(model);
            if (item.HasGeometryColumn) {
                var featureProvider = factory.CreateFeatureProvider(item.DatabaseType);
                item.Srid = await featureProvider.GetSridAsync(item);
                item.GeometryType = await featureProvider.GetGeometryTypeAsync(item);
            }
            await cache.SetAsync(key, item);
            return item;
        }

    }

}
