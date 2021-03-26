using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.VectorTile.Data;
using Beginor.GisHub.VectorTile.Models;

namespace Beginor.GisHub.VectorTile.Data {

    /// <summary>矢量切片包仓储实现</summary>
    public partial class VectortileRepository : HibernateRepository<Vectortile, VectortileModel, long>, IVectortileRepository {

        public VectortileRepository(ISession session, IMapper mapper) : base(session, mapper) { }

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


        public override async Task DeleteAsync(long id, CancellationToken token = default) {
            var entity = Session.Get<Vectortile>(id);
            if (entity != null) {
                entity.IsDeleted = true;
                await Session.SaveAsync(entity, token);
                await Session.FlushAsync();
            }
        }
    }

}
