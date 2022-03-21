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
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories; 

/// <summary>导航节点（菜单）仓储实现</summary>
public partial class AppNavItemRepository : HibernateRepository<AppNavItem, AppNavItemModel, long>, IAppNavItemRepository {

    private UserManager<AppUser> userManager;

    public AppNavItemRepository(
        ISession session,
        IMapper mapper,
        UserManager<AppUser> userManager
    ) : base(session, mapper) {
        this.userManager = userManager;
    }

    protected override void Dispose(
        bool disposing
    ) {
        base.Dispose(disposing);
        if (disposing) {
            userManager = null;
        }
    }

    public override async Task DeleteAsync(
        long id,
        CancellationToken token = default(CancellationToken)
    ) {
        var entity = await Session.GetAsync<AppNavItem>(id, token);
        if (entity == null) {
            return;
        }
        entity.IsDeleted = true;
        await Session.UpdateAsync(entity, token);
        await Session.FlushAsync(token);
        Session.Clear();
    }

    public async Task<PaginatedResponseModel<AppNavItemModel>> SearchAsync(
        AppNavItemSearchModel model
    ) {
        var query = Session.Query<AppNavItem>();
        // todo: add custom query here;
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppNavItemModel> {
            Total = total,
            Data = Mapper.Map<IList<AppNavItemModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task SaveAsync(AppNavItemModel model, string userName) {
        // Check parameters;
        Argument.NotNull(model, nameof(model));
        Argument.NotNullOrEmpty(userName, nameof(userName));
        // Map to entity;
        var entity = Mapper.Map<AppNavItem>(model);
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
        AppNavItemModel model,
        string userName
    ) {
        // Check parameters;
        if (id <= 0) {
            throw new ArgumentOutOfRangeException(nameof(id), "NavItemId <= 0 !");
        }
        Argument.NotNull(model, nameof(model));
        Argument.NotNullOrEmpty(userName, nameof(userName));
        var entity = await Session.LoadAsync<AppNavItem>(id);
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

    public async Task<MenuNodeModel> GetMenuAsync(string[] roles) {
        if (roles == null) {
            throw new ArgumentNullException(nameof(roles), "roles can not be null.");
        }
        var conn = Session.Connection;
        var sql = @"
                with recursive cte as (
                    select p.id, p.parent_id, p.title, p.tooltip, p.icon, p.url, p.sequence, p.roles, p.target, p.frame_url, p.is_hidden
                    from public.app_nav_items p
                    where p.parent_id is null and p.is_deleted = false
                    union all
                    select c.id, c.parent_id, c.title, c.tooltip, c.icon, c.url, c.sequence, c.roles, c.target, c.frame_url, c.is_hidden
                    from public.app_nav_items c
                    inner join cte on cte.id = c.parent_id
                    where (c.is_deleted = false) and (c.roles && @roles::character varying[])
                )
                select * from cte;
            ";
        var navItems = await conn.QueryAsync<AppNavItem>(sql, new { roles });
        var rootNavItem = navItems.OrderBy(n => n.Id).First(n => n.ParentId == null);
        var model = new MenuNodeModel {
            Id = rootNavItem.Id.ToString(),
            Title = rootNavItem.Title,
            Url = rootNavItem.Url,
            Icon = rootNavItem.Icon,
            Tooltip = rootNavItem.Tooltip,
            IsHidden = false,
            Children = FindChildrenRecursive(rootNavItem.Id, navItems)
        };
        return model;
    }

    private MenuNodeModel[] FindChildrenRecursive(long id, IEnumerable<AppNavItem> items) {
        var childCount = items.Count(item => item.ParentId == id);
        if (childCount == 0) {
            return null;
        }
        var children = items.Where(item => item.ParentId == id)
            .OrderBy(item => item.Sequence)
            .Select(item => new MenuNodeModel {
                Id = item.Id.ToString(),
                Title = item.Title,
                Url = item.Url,
                Icon = item.Icon,
                Tooltip = item.Tooltip,
                Target = item.Target,
                FrameUrl = item.FrameUrl,
                IsHidden = item.IsHidden,
                Children = FindChildrenRecursive(item.Id, items)
            });
        return children.ToArray();
    }

}