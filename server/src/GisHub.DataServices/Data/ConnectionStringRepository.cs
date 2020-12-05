using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.DataServices.Models;

namespace Beginor.GisHub.DataServices.Data {

    /// <summary>数据库连接串仓储实现</summary>
    public partial class ConnectionStringRepository : HibernateRepository<ConnectionString, ConnectionStringModel, long>, IConnectionStringRepository {

        public ConnectionStringRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 数据库连接串 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ConnectionStringModel>> SearchAsync(
            ConnectionStringSearchModel model
        ) {
            var query = Session.Query<ConnectionString>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<ConnectionStringModel> {
                Total = total,
                Data = Mapper.Map<IList<ConnectionStringModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}
