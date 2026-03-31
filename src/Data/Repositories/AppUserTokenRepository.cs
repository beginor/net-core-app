using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>用户凭证仓储实现</summary>
public partial class AppUserTokenRepository : HibernateRepository<AppUserTokenEntity, AppUserTokenModel, long>, IAppUserTokenRepository {

    private IDistributedCache cache;
    private CommonOption commonOption;

    public AppUserTokenRepository(ISession session, IMapper mapper, IDistributedCache cache, CommonOption commonOption) : base(session, mapper) {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.commonOption = commonOption ?? throw new ArgumentNullException(nameof(commonOption));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 用户凭证 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppUserTokenModel>> SearchAsync(
        AppUserTokenSearchModel model,
        string userId = ""
    ) {
        var query = Session.Query<AppUserTokenEntity>();
        if (userId.IsNotNullOrEmpty()) {
            query = query.Where(e => e.User!.Id == userId);
        }
        var keywords = model.Keywords;
        if (keywords.IsNotNullOrEmpty()) {
            query = query.Where(e => e.Name!.Contains(keywords!) || e.Value == keywords);
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

    public async Task<AppUserTokenEntity?> GetTokenByValueAsync(string tokenValue) {
        var key = string.Format(CacheKeyFormat.UserToken, tokenValue);
        var entity = await cache.GetAsync<AppUserTokenEntity>(key);
        if (entity == null) {
            entity = await Session.Query<AppUserTokenEntity>()
                .Where(tkn => tkn.Value == tokenValue)
                .Select(tk => new AppUserTokenEntity {
                    Id = tk.Id,
                    Name = tk.Name,
                    Value = tk.Value,
                    ExpiresAt = tk.ExpiresAt,
                    UpdateTime = tk.UpdateTime,
                    Roles = tk.Roles,
                    Privileges = tk.Privileges,
                    Urls = tk.Urls,
                    User = new AppUserEntity {
                        Id = tk.User.Id,
                        UserName = tk.User.UserName,
                        Email = tk.User.Email,
                        LockoutEnabled = tk.User.LockoutEnabled,
                        LockoutEndUnixTimeSeconds = tk.User.LockoutEndUnixTimeSeconds
                    }
                }).FirstOrDefaultAsync();
            if (entity != null) {
                await cache.SetAsync<AppUserTokenEntity>(
                    string.Format(CacheKeyFormat.UserToken, entity.Value!),
                    entity,
                    commonOption.Cache.MemoryExpiration
                );
            }
        }
        return entity;
    }

    public async Task<AppUserTokenModel?> GetTokenForUserAsync(long id, string userId) {
        var entity = await Session.Query<AppUserTokenEntity>()
            .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User!.Id == userId);
        return entity == null ? null : Mapper.Map<AppUserTokenModel>(entity);
    }

    public async Task SaveTokenForUserAsync(AppUserTokenModel model, AppUserEntity user) {
        var entity = Mapper.Map<AppUserTokenEntity>(model);
        entity.User = user;
        entity.UpdateTime = DateTime.Now;
        await Session.SaveAsync(entity);
        await Session.FlushAsync();
        Session.Clear();
        Mapper.Map(entity, model);
    }

    public async Task<bool> ExistsAsync(long id, string userId) {
        return await Session.Query<AppUserTokenEntity>()
            .AnyAsync(tkn => tkn.Id == id && tkn.User!.Id == userId);
    }

    public async Task UpdateTokenForUserAsync(long id, AppUserTokenModel model, AppUserEntity user) {
        var entity = await Session.Query<AppUserTokenEntity>()
            .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User!.Id == user.Id);
        if (entity == null) {
            throw new Exception($"entity AppUserToken with id {id} is null");
        }
        await cache.RemoveAsync(entity.Value!);
        Mapper.Map(model, entity);
        entity.User = user;
        entity.UpdateTime = DateTime.Now;
        await Session.UpdateAsync(entity);
        await Session.FlushAsync();
        Session.Clear();
        Mapper.Map(entity, model);
    }

    public async Task DeleteTokenForUserAsync(long id, string userId) {
        var entity = await Session.Query<AppUserTokenEntity>()
            .FirstOrDefaultAsync(tkn => tkn.Id == id && tkn.User!.Id == userId);
        if (entity != null) {
            await cache.RemoveAsync(entity.Value!);
            await Session.DeleteAsync(entity);
            await Session.FlushAsync();
            Session.Clear();
        }
    }

}
