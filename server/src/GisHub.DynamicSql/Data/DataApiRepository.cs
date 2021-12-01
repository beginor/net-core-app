using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Common;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DynamicSql;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据API仓储实现</summary>
    public partial class DataApiRepository : HibernateRepository<DataApi, DataApiModel, long>, IDataApiRepository {

        private IDynamicSqlProvider dynamicSqlProvider;
        private IDistributedCache cache;
        private CommonOption commonOption;
        private IDataServiceFactory dataServiceFactory;
        private ILogger<DataApiRepository> logger;

        public DataApiRepository(
            ISession session,
            IMapper mapper,
            IDynamicSqlProvider dynamicSqlProvider,
            IDistributedCache cache,
            CommonOption commonOption,
            IDataServiceFactory dataServiceFactory,
            ILogger<DataApiRepository> logger
        ) : base(session, mapper) {
            this.dynamicSqlProvider = dynamicSqlProvider ?? throw new ArgumentNullException(nameof(dynamicSqlProvider));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
            this.dataServiceFactory = dataServiceFactory ?? throw new ArgumentNullException(nameof(dataServiceFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                dynamicSqlProvider = null;
                cache = null;
                commonOption = null;
                dataServiceFactory = null;
                logger = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 数据API ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<DataApiModel>> SearchAsync(
            DataApiSearchModel model
        ) {
            var query = Session.Query<DataApi>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.Select(x => new DataApi {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    DataSource = x.DataSource,
                    WriteData = x.WriteData,
                    Roles = x.Roles,
                    Creator = x.Creator,
                    CreatedAt = x.CreatedAt,
                    Updater = x.Updater,
                    UpdatedAt = x.UpdatedAt,
                    IsDeleted = x.IsDeleted
                })
                .OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<DataApiModel> {
                Total = total,
                Data = Mapper.Map<IList<DataApiModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }


        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<DataApi>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync(token);
                Session.Clear();
                await cache.RemoveAsync(id.ToString());
            }
        }

        public async Task DeleteAsync(long id, AppUser user, CancellationToken token = default) {
            var entity = Session.Get<DataApi>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                entity.Updater = user;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync(token);
                Session.Clear();
                await cache.RemoveAsync(id.ToString());
            }
        }

        public async Task SaveAsync(DataApiModel model, AppUser user, CancellationToken token = default) {
            var entity = Mapper.Map<DataApi>(model);
            entity.Creator = user;
            entity.CreatedAt = DateTime.Now;
            entity.Updater = user;
            entity.UpdatedAt = DateTime.Now;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            Session.Clear();
            Mapper.Map(entity, model);
        }

        public async Task UpdateAsync(long id, DataApiModel model, AppUser user, CancellationToken token = default) {
            var entity = await Session.LoadAsync<DataApi>(id, token);
            Mapper.Map(model, entity);
            entity.Statement.DocumentElement?.SetAttribute("Id", id.ToString());
            entity.Updater = user;
            entity.UpdatedAt = DateTime.Now;
            await Session.UpdateAsync(entity, token);
            await Session.FlushAsync(token);
            Session.Clear();
            await cache.RemoveAsync(id.ToString());
        }

    }

}
