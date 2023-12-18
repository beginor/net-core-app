using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>组织单元仓储实现</summary>
public partial class AppOrganizeUnitRepository(
    ISession session,
    IMapper mapper
) : HibernateRepository<AppOrganizeUnit, AppOrganizeUnitModel, long>(session, mapper),
    IAppOrganizeUnitRepository {

    /// <summary>搜索 组织单元 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppOrganizeUnitModel>> SearchAsync(
        AppOrganizeUnitSearchModel model,
        ClaimsPrincipal user
    ) {
        var userUnitId = user.GetOrganizeUnitId();
        var unitId = userUnitId;
        if (model.OrganizeUnitId.HasValue) {
            var value = model.OrganizeUnitId.Value;
            var canView = await CanViewOrganizeUnitAsync(userUnitId, value);
            if (!canView) {
                throw new InvalidOperationException($"User {user.GetUserName()} can not access organize unit {unitId}");
            }
            unitId = value;
        }
        var conn = Session.Connection;
        var sql = @"
            with recursive cte as (
                select p.id, p.parent_id, p.code, p.name, p.description, p.sequence, 0 as level, true as expand
                from public.app_organize_units p
                where p.is_deleted = false and p.id = @unitId
                union all
                select c.id, c.parent_id,c.code,c.name,c.description,c.sequence, (cte.level + 1) as level, false as expand
                from public.app_organize_units c
                inner join cte on cte.id = c.parent_id
                where c.is_deleted = false
            )
            select id, parent_id, code, name, description, sequence, level, expand
            from cte a
            order by a.code, a.sequence;
        ";
        var models = await conn.QueryAsync<AppOrganizeUnit>(sql, new { unitId });
        var result = new PaginatedResponseModel<AppOrganizeUnitModel> {
            Data = Mapper.Map<IList<AppOrganizeUnitModel>>(models.ToList())
        };
        return result;
    }

    public async Task<AppOrganizeUnitModel> GetByIdAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken token = default
    ) {
        var entity = await Session.GetAsync<AppOrganizeUnit>(id, token);
        if (entity == null) {
            throw new InvalidOperationException($"Organize unit {id} does not exist!");
        }
        var canView = await CanViewOrganizeUnitAsync(user.GetOrganizeUnitId(), id, token);
        if (!canView) {
            throw new InvalidOperationException($"User {user.GetUserName()} can not access organize unit {id}");
        }
        return Mapper.Map<AppOrganizeUnitModel>(entity);
    }

    public override async Task DeleteAsync(
        long id,
        CancellationToken token = default
    ) {
        var entity = await Session.GetAsync<AppOrganizeUnit>(id, token);
        if (entity != null) {
            entity.IsDeleted = true;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
        }
    }

    public async Task DeleteAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken token = default
    ) {
        var entity = await Session.GetAsync<AppOrganizeUnit>(id, token);
        if (entity == null) {
            return;
        }
        var canView = await CanViewOrganizeUnitAsync(user.GetOrganizeUnitId(), id, token);
        if (!canView) {
            throw new InvalidOperationException($"User {user.GetUserName()} can not access organize unit {id}");
        }
        entity.Updater = await Session.GetAsync<AppUser>(user.GetUserId(), token);
        entity.UpdatedAt = DateTime.Now;
        await Session.SaveAsync(entity, token);
        await Session.FlushAsync(token);
    }

    public async Task SaveAsync(
        AppOrganizeUnitModel model,
        ClaimsPrincipal user,
        CancellationToken token = default
    ) {
        // Check parameters;
        Argument.NotNull(model, nameof(model));
        Argument.NotNull(user, nameof(user));
        if (!long.TryParse(model.ParentId, out var parentId)) {
            throw new InvalidOperationException($"Invalid parent organize unit id {model.ParentId}");
        }
        var canView = await CanViewOrganizeUnitAsync(user.GetOrganizeUnitId(), parentId, token);
        if (!canView) {
            throw new InvalidOperationException($"User {user.GetUserName()} can not access organize unit {parentId}");
        }
        // Map to entity;
        var entity = Mapper.Map<AppOrganizeUnit>(model);
        var tx = Session.BeginTransaction();
        try {
            var creator = await Session.GetAsync<AppUser>(user.GetUserId(), token);
            // Assign creator;
            entity.Creator = creator;
            entity.CreatedAt = DateTime.Now;
            // Assign updater;
            entity.Updater = creator;
            entity.UpdatedAt = DateTime.Now;
            // Ensure not deleted.
            entity.IsDeleted = false;
            await Session.SaveAsync(entity, token);
            entity.Code = await GenerateUnitCodeAsync(entity.Id, token);
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            Session.Clear();
            await tx.CommitAsync(token);
            Mapper.Map(entity, model);
        }
        catch (Exception) {
            await tx.RollbackAsync(token);
        }
    }

    public async Task UpdateAsync(
        long id,
        AppOrganizeUnitModel model,
        ClaimsPrincipal user,
        CancellationToken token = default
    ) {
        // Check parameters;
        if (id <= 0) {
            throw new ArgumentOutOfRangeException(nameof(id), "Id <= 0 !");
        }
        Argument.NotNull(model, nameof(model));
        Argument.NotNull(user, nameof(user));
        var entity = await Session.LoadAsync<AppOrganizeUnit>(id, token);
        if (entity == null) {
            throw new InvalidOperationException($"组织单元 {id} 不存在！");
        }
        var canView = await CanViewOrganizeUnitAsync(user.GetOrganizeUnitId(), entity.Id, token);
        if (!canView) {
            throw new InvalidOperationException($"User {user.GetUserName()} can not access organize unit {entity.Id}");
        }
        var trans = Session.BeginTransaction();
        try {
            var oldParentId = entity.ParentId;
            var oldCode = entity.Code;
            Mapper.Map(model, entity);
            entity.Id = id;
            var updater = await Session.GetAsync<AppUser>(user.GetUserId(), token);
            entity.Updater = updater;
            entity.UpdatedAt = DateTime.Now;
            await Session.UpdateAsync(entity, token);
            if (entity.ParentId != oldParentId) {
                var descendants = await Session.Query<AppOrganizeUnit>()
                .Where(x => x.Code.StartsWith($"{oldCode}/"))
                .ToListAsync(cancellationToken: token);
                entity.UpdatedAt = DateTime.Now;
                entity.Code = await GenerateUnitCodeAsync(entity.Id, token);
                await Session.UpdateAsync(entity, token);
                foreach (var descendant in descendants) {
                    descendant.Code = await GenerateUnitCodeAsync(descendant.Id, token);
                    descendant.Updater = updater;
                    descendant.UpdatedAt = DateTime.Now;
                    await Session.UpdateAsync(descendant, token);
                }
            }
            await Session.FlushAsync(token);
            Session.Clear();
            await trans.CommitAsync(token);
        }
        catch (Exception) {
            await trans.RollbackAsync(token);
            throw;
        }
    }

    public async Task<IList<AppOrganizeUnitModel>> QueryPathAsync(
        long unitId,
        CancellationToken token = default
    ) {
        var entities = await QueryUnitPathAsync(unitId, token);
        return Mapper.Map<IList<AppOrganizeUnitModel>>(entities);
    }

    public async Task<bool> CanViewOrganizeUnitAsync(
        long userUnitId,
        long unitId,
        CancellationToken token = default
    ) {
        var units = await QueryUnitPathAsync(unitId, token);
        return units.Any(unit => unit.Id == userUnitId);
    }

    private async Task<IList<AppOrganizeUnit>> QueryUnitPathAsync(
        long unitId,
        CancellationToken token = default
    ) {
        var conn = Session.Connection;
        var sql = @"
            with recursive cte as (
                select c.id, c.parent_id,c.code,c.name,c.description,c.sequence
                from public.app_organize_units c
                where c.is_deleted = false and c.id = @unitId
                union all
                select p.id, p.parent_id, p.code, p.name, p.description, p.sequence
                from public.app_organize_units p
                inner join cte on cte.parent_id = p.id
                where p.is_deleted = false
            ) select * from cte
        ";
        var units = await conn.QueryAsync<AppOrganizeUnit>(sql, new { unitId });
        return units.ToList();
    }

    public async Task<AppOrganizeUnit> GetEntityByIdAsync(
        long unitId,
        ClaimsPrincipal user,
        CancellationToken token = default
    ) {
        var entity = await Session.LoadAsync<AppOrganizeUnit>(unitId, token);
        return entity;
    }

    private async Task<string> GenerateUnitCodeAsync(
        long id,
        CancellationToken token = default
    ) {
        var units = await QueryUnitPathAsync(id, token);
        var idArr = units.Select(x => x.Id.ToString()).ToArray();
        Array.Reverse(idArr);
        var path = string.Join('/', idArr);
        return path;
    }

}
