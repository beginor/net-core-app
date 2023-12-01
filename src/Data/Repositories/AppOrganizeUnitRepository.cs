using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>组织单元仓储实现</summary>
public partial class AppOrganizeUnitRepository : HibernateRepository<AppOrganizeUnit, AppOrganizeUnitModel, long>, IAppOrganizeUnitRepository {

    private UserManager<AppUser> userManager;

    public AppOrganizeUnitRepository(
        ISession session,
        IMapper mapper,
        UserManager<AppUser> userManager
    ) : base(session, mapper) {
        this.userManager = userManager;
    }

    /// <summary>搜索 组织单元 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppOrganizeUnitModel>> SearchAsync(
        AppOrganizeUnitSearchModel model
    ) {
        var query = Session.Query<AppOrganizeUnit>();
        // todo: 添加自定义查询；
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppOrganizeUnitModel> {
            Total = total,
            Data = Mapper.Map<IList<AppOrganizeUnitModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public override async Task DeleteAsync(long id, CancellationToken token = default) {
        var entity = Session.Get<AppOrganizeUnit>(id);
        if (entity != null) {
            entity.IsDeleted = true;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
        }
    }

    public async Task SaveAsync(AppOrganizeUnitModel model, string userName) {
        // Check parameters;
        Argument.NotNull(model, nameof(model));
        Argument.NotNullOrEmpty(userName, nameof(userName));
        // Map to entity;
        var entity = Mapper.Map<AppOrganizeUnit>(model);
        var user = await Session.Query<AppUser>()
            .FirstOrDefaultAsync(
                u => u.UserName == userName
            );
        // Assign creator;
        entity.Creator = user;
        entity.CreatedAt = DateTime.Now;
        // Assign updater;
        entity.Updater = user;
        entity.UpdatedAt = DateTime.Now;
        // Ensure not deleted.
        entity.IsDeleted = false;
        await Session.SaveAsync(entity);
        await Session.FlushAsync();
        Session.Clear();
        Mapper.Map(entity, model);
    }

    public async Task UpdateAsync(
        long id,
        AppOrganizeUnitModel model,
        string userName
    ) {
        // Check parameters;
        if (id <= 0) {
            throw new ArgumentOutOfRangeException(nameof(id), "NavItemId <= 0 !");
        }
        Argument.NotNull(model, nameof(model));
        Argument.NotNullOrEmpty(userName, nameof(userName));
        var entity = await Session.LoadAsync<AppOrganizeUnit>(id);
        if (entity == null) {
            throw new InvalidOperationException(
                $"导航节点 {id} 不存在！"
            );
        }
        Mapper.Map(model, entity);
        entity.Id = id;
        entity.Updater = await Session.Query<AppUser>()
            .FirstAsync(u => u.UserName == userName);
        entity.UpdatedAt = DateTime.Now;
        await Session.UpdateAsync(entity);
        await Session.FlushAsync();
        Session.Clear();
    }

}
