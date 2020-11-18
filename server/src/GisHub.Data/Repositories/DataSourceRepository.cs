using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>数据源（数据表或视图）仓储实现</summary>
    public partial class DataSourceRepository : HibernateRepository<DataSource, DataSourceModel, long>, IDataSourceRepository {

        public DataSourceRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 数据源（数据表或视图） ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<DataSourceModel>> SearchAsync(
            DataSourceSearchModel model
        ) {
            var query = Session.Query<DataSource>();
            // todo: 添加自定义查询；
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

    }

}