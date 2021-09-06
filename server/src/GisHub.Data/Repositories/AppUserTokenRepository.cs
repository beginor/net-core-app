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
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    /// <summary>用户凭证仓储实现</summary>
    public partial class AppUserTokenRepository : HibernateRepository<AppUserToken, AppUserTokenModel, long>, IAppUserTokenRepository {

        private IDistributedCache cache;

        public AppUserTokenRepository(ISession session, IMapper mapper, IDistributedCache cache) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

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

        public async Task<AppUserToken> GetTokenByValue(string tokenValue) {
            var token = await cache.GetAsync<AppUserToken>(tokenValue);
            if (token == null) {
                token = await Session.Query<AppUserToken>()
                    .Where(tk => tk.Value == tokenValue)
                    .Select(tk => new AppUserToken {
                        Id = tk.Id,
                        User = new AppUser {
                            Id = tk.User.Id,
                            UserName = tk.User.UserName
                        },
                        Name = tk.Name,
                        Value = tk.Value,
                        Privileges = tk.Privileges,
                        Urls = tk.Urls,
                        ExpiresAt = tk.ExpiresAt,
                        UpdateTime = tk.UpdateTime
                    })
                    .FirstOrDefaultAsync();
                if (token != null) {
                    await cache.SetAsync<AppUserToken>(token.Value, token);
                }
            }
            return token;
        }

    }

}
