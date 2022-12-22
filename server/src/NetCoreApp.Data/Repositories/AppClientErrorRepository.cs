using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>程序客户端错误记录仓储实现</summary>
public partial class AppClientErrorRepository : HibernateRepository<AppClientError, AppClientErrorModel, long>, IAppClientErrorRepository {

    public AppClientErrorRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    /// <summary>搜索 程序客户端错误记录 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppClientErrorModel>> SearchAsync(
        AppClientErrorSearchModel model
    ) {
        var query = Session.Query<AppClientError>();
        // todo: 添加自定义查询；
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppClientErrorModel> {
            Total = total,
            Data = Mapper.Map<IList<AppClientErrorModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

}
