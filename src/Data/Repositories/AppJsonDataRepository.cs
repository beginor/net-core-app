using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>json 数据仓储实现</summary>
public partial class AppJsonDataRepository : HibernateRepository<AppJsonData, AppJsonDataModel, long>, IAppJsonDataRepository {

    public AppJsonDataRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    /// <summary>搜索 json 数据 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppJsonDataModel>> SearchAsync(
        AppJsonDataSearchModel model
    ) {
        var query = Session.Query<AppJsonData>();
        var businessId = model.BusinessId;
        if (businessId > 0) {
            query = query.Where(x => x.BusinessId == businessId);
        }
        var name = model.Name;
        if (!string.IsNullOrEmpty(name)) {
            query = query.Where(x => x.Name.Contains(name));
        }
        var total = await query.LongCountAsync();
        var data = await query.Select(x => new AppJsonData {
                Id = x.Id,
                BusinessId = x.BusinessId,
                Name = x.Name,
            }).OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        var emptyElement = JsonDocument.Parse("{}").RootElement;
        foreach (var item in data) {
            item.Value = emptyElement;
        }
        return new PaginatedResponseModel<AppJsonDataModel> {
            Total = total,
            Data = Mapper.Map<IList<AppJsonDataModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public async Task<AppJsonData?> GetByBusinessIdAsync(long businessId) {
        var jsonData = await Session.Query<AppJsonData>().FirstOrDefaultAsync(
            x => x.BusinessId == businessId
        );
        return jsonData;
    }

}
