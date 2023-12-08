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
        var unitId = model.OrganizeUnitId ?? 0L;
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
        var tx = Session.BeginTransaction();
        try {
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
            entity.Code = await GenerateUnitCodeAsync(entity.Id);
            await Session.SaveAsync(entity);
            await Session.FlushAsync();
            Session.Clear();
            await tx.CommitAsync();
            Mapper.Map(entity, model);
        }
        catch (Exception) {
            await tx.RollbackAsync();
        }
    }

    public async Task UpdateAsync(
        long id,
        AppOrganizeUnitModel model,
        string userName
    ) {
        // Check parameters;
        if (id <= 0) {
            throw new ArgumentOutOfRangeException(nameof(id), "Id <= 0 !");
        }
        Argument.NotNull(model, nameof(model));
        Argument.NotNullOrEmpty(userName, nameof(userName));
        var entity = await Session.LoadAsync<AppOrganizeUnit>(id);
        if (entity == null) {
            throw new InvalidOperationException(
                $"组织单元 {id} 不存在！"
            );
        }
        var trans = Session.BeginTransaction();
        try {
            var oldParentId = entity.ParentId;
            var oldCode = entity.Code;
            Mapper.Map(model, entity);
            entity.Id = id;
            var updater = await Session.Query<AppUser>()
                .FirstAsync(u => u.UserName == userName);
            entity.Updater = updater;
            entity.UpdatedAt = DateTime.Now;
            await Session.UpdateAsync(entity);
            if (entity.ParentId != oldParentId) {
                var descendants = await Session.Query<AppOrganizeUnit>()
                .Where(x => x.Code.StartsWith($"{oldCode}/"))
                .ToListAsync();
                entity.UpdatedAt = DateTime.Now;
                entity.Code = await GenerateUnitCodeAsync(entity.Id);
                await Session.UpdateAsync(entity);
                foreach (var descendant in descendants) {
                    descendant.Code = await GenerateUnitCodeAsync(descendant.Id);
                    descendant.Updater = updater;
                    descendant.UpdatedAt = DateTime.Now;
                    await Session.UpdateAsync(descendant);
                }
            }
            await Session.FlushAsync();
            Session.Clear();
            await trans.CommitAsync();
        }
        catch (Exception) {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<IList<AppOrganizeUnitModel>> QueryPathAsync(long unitId) {
        var entities = await QueryUnitPathAsync(unitId);
        return Mapper.Map<IList<AppOrganizeUnitModel>>(entities);
    }

    public async Task<bool> CanViewOrganizeUnitAsync(long userUnitId, long unitId) {
        var units = await QueryUnitPathAsync(unitId);
        return units.Any(unit => unit.Id == userUnitId);
    }

    private async Task<IList<AppOrganizeUnit>> QueryUnitPathAsync(long unitId) {
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

    public async Task<AppOrganizeUnit> GetEntityByIdAsync(long unitId) {
        var entity = await Session.LoadAsync<AppOrganizeUnit>(unitId);
        return entity;
    }

    public async Task<string> GenerateUnitCodeAsync(long id) {
        var units = await QueryUnitPathAsync(id);
        var idArr = units.Select(x => x.Id.ToString()).ToArray();
        Array.Reverse(idArr);
        var path = string.Join('/', idArr);
        return path;
    }

}
