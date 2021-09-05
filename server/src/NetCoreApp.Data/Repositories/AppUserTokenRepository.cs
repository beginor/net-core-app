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
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>用户凭证仓储实现</summary>
    public partial class AppUserTokenRepository : HibernateRepository<AppUserToken, AppUserTokenModel, long>, IAppUserTokenRepository {

        public AppUserTokenRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 用户凭证 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppUserTokenModel>> SearchAsync(
            AppUserTokenSearchModel model
        ) {
            var query = Session.Query<AppUserToken>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<AppUserTokenModel> {
                Total = total,
                Data = Mapper.Map<IList<AppUserTokenModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}