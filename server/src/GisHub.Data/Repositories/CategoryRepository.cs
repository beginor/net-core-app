using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>数据类别仓储实现</summary>
public partial class CategoryRepository : HibernateRepository<Category, CategoryModel, long>, ICategoryRepository {

    public CategoryRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    /// <summary>搜索 数据类别 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<CategoryModel>> SearchAsync(
        CategorySearchModel model
    ) {
        var query = Session.Query<Category>();
        // todo: 添加自定义查询；
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<CategoryModel> {
            Total = total,
            Data = Mapper.Map<IList<CategoryModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

    public override async Task<IList<CategoryModel>> GetAllAsync(CancellationToken token = new()) {
        var query = Session.Query<Category>()
            .OrderBy(e => e.Id)
            .ThenBy(e => e.ParentId)
            .ThenBy(e => e.Sequence);
        var data = await query.ToListAsync(token);
        return Mapper.Map<IList<CategoryModel>>(data);
    }

}
