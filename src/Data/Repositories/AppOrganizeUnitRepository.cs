using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>组织单元仓储实现</summary>
public partial class AppOrganizeUnitRepository : HibernateRepository<AppOrganizeUnit, AppOrganizeUnitModel, long>, IAppOrganizeUnitRepository {

    public AppOrganizeUnitRepository(ISession session, IMapper mapper) : base(session, mapper) { }

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
            await Session.FlushAsync();
        }
    }

}
