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
            AppUserTokenSearchModel model,
            string userId = ""
        ) {
            var query = Session.Query<AppUserToken>();
            if (userId.IsNotNullOrEmpty()) {
                query = query.Where(e => e.User.Id == userId);
            }
            var keywords = model.Keywords;
            if (keywords.IsNotNullOrEmpty()) {
                query = query.Where(e => e.Name.Contains(keywords));
            }
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

        public async Task<AppUserToken> GetTokenByValueAsync(string tokenValue) {
            var entity = await cache.GetAsync<AppUserToken>(tokenValue);
            if (entity == null) {
                entity = await Session.Query<AppUserToken>()
                    .Where(tkn => tkn.Value == tokenValue)
                    .FirstOrDefaultAsync();
                if (entity != null) {
                    await cache.SetAsync<AppUserToken>(entity.Value, entity);
                }
            }
            return entity;
        }

        public async Task<AppUserTokenModel> GetTokenForUserAsync(long id, string userId) {
            var entity = await Session.Query<AppUserToken>()
                .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User.Id == userId);
            return entity == null ? null : Mapper.Map<AppUserTokenModel>(entity);
        }

        public async Task SaveTokenForUserAsync(AppUserTokenModel model, AppUser user) {
            var entity = Mapper.Map<AppUserToken>(model);
            entity.User = user;
            entity.UpdateTime = DateTime.Now;
            await Session.SaveAsync(entity);
            await Session.FlushAsync();
            Session.Clear();
            Mapper.Map(entity, model);
        }

        public async Task<bool> ExistsAsync(long id, string userId) {
            return await Session.Query<AppUserToken>()
                .AnyAsync(tkn => tkn.Id == id && tkn.User.Id == userId);
        }

        public async Task UpdateTokenForUserAsync(long id, AppUserTokenModel model, AppUser user) {
            var entity = await Session.Query<AppUserToken>()
                .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User.Id == user.Id);
            if (entity == null) {
                throw new Exception($"entity AppUserToken with id {id} is null");
            }
            await cache.RemoveAsync(entity.Value);
            Mapper.Map(model, entity);
            entity.User = user;
            entity.UpdateTime = DateTime.Now;
            await Session.UpdateAsync(entity);
            await Session.FlushAsync();
            Session.Clear();
            Mapper.Map(entity, model);
        }

        public async Task DeleteTokenForUserAsync(long id, string userId) {
            var entity = await Session.Query<AppUserToken>()
                .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User.Id == userId);
            if (entity != null) {
                await cache.RemoveAsync(entity.Value);
                await Session.DeleteAsync(entity);
                await Session.FlushAsync();
                Session.Clear();
            }
        }

    }

}
