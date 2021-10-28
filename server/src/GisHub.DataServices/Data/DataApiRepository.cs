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

    /// <summary>数据API仓储实现</summary>
    public partial class DataApiRepository : HibernateRepository<DataApi, DataApiModel, long>, IDataApiRepository {

        public DataApiRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 数据API ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<DataApiModel>> SearchAsync(
            DataApiSearchModel model
        ) {
            var query = Session.Query<DataApi>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
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
                await Session.FlushAsync();
            }
        }
    }

}
