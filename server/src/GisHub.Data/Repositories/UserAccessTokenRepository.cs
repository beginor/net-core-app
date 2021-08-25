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
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>用户访问凭证仓储实现</summary>
    public partial class UserAccessTokenRepository : HibernateRepository<UserAccessToken, UserAccessTokenModel, long>, IUserAccessTokenRepository {

        public UserAccessTokenRepository(ISession session, IMapper mapper) : base(session, mapper) { }

        /// <summary>搜索 用户访问凭证 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<UserAccessTokenModel>> SearchAsync(
            UserAccessTokenSearchModel model
        ) {
            var query = Session.Query<UserAccessToken>();
            // todo: 添加自定义查询；
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<UserAccessTokenModel> {
                Total = total,
                Data = Mapper.Map<IList<UserAccessTokenModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}