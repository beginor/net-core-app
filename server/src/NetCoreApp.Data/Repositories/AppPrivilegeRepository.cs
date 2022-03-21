using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories; 

/// <summary>系统权限仓储实现</summary>
public partial class AppPrivilegeRepository : HibernateRepository<AppPrivilege, AppPrivilegeModel, long>, IAppPrivilegeRepository {

    public AppPrivilegeRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    public async Task<PaginatedResponseModel<AppPrivilegeModel>> SearchAsync(
        AppPrivilegeSearchModel model
    ) {
        var query = Session.Query<AppPrivilege>();
        if (!string.IsNullOrEmpty(model.Module)) {
            query = query.Where(p => p.Module == model.Module);
        }
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppPrivilegeModel> {
            Total = total,
            Data = Mapper.Map<IList<AppPrivilegeModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    /// <summary>返回权限表的所有模块</summary>
    public async Task<IList<string>> GetModulesAsync() {
        var modules = await Session.Query<AppPrivilege>()
            .Select(p => p.Module)
            .Distinct()
            .ToListAsync();
        return modules;
    }

    /// <summary>同步必须的权限</summary>
    public async Task SyncRequiredAsync(IEnumerable<string> names) {
        foreach (var name in names) {
            var exists = await Session.Query<AppPrivilege>()
                .AnyAsync(e => e.Name == name);
            if (exists) {
                continue;
            }
            var entity = new AppPrivilege {
                Name = name,
                Module = name.Substring(0, name.IndexOf('.')),
                Description = string.Empty,
                IsRequired = true
            };
            await Session.SaveAsync(entity);
        }
    }

    public override async Task DeleteAsync(
        long id,
        CancellationToken token = new CancellationToken()
    ) {
        var tx = Session.BeginTransaction();
        try {
            var entity = await Session.LoadAsync<AppPrivilege>(id, token);
            if (entity == null) {
                return;
            }
            if (entity.IsRequired) {
                throw new InvalidOperationException("无法删除必须的权限！");
            }
            await Session.DeleteAsync(entity, token);
            // delete privileges in role claims;
            var claims = await Session.Query<IdentityRoleClaim>()
                .Where(c => c.ClaimValue == entity.Name && c.ClaimType == Consts.PrivilegeClaimType)
                .ToListAsync();
            foreach (var claim in claims) {
                await Session.DeleteAsync(claim, token);
            }
            await Session.FlushAsync(token);
            Session.Clear();
            await tx.CommitAsync();
        }
        catch (Exception) {
            tx.Rollback();
            throw;
        }
    }

    public async Task<IList<AppPrivilegeModel>> GetByNamesAsync(IList<string> names) {
        var entities = await Session.Query<AppPrivilege>()
            .Where(p => names.Contains(p.Name))
            .ToListAsync();
        return Mapper.Map<IList<AppPrivilegeModel>>(entities);
    }

}