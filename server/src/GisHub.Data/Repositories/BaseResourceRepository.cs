using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;
using Dapper;

namespace Beginor.GisHub.Data.Repositories;

/// <summary>数据资源的基类仓储实现</summary>
public partial class BaseResourceRepository : Disposable, IBaseResourceRepository {

    private ISession session;
    private IMapper mapper;

    public BaseResourceRepository(ISession session, IMapper mapper) {
        this.session = session ?? throw new ArgumentNullException(nameof(session));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            session = null;
            mapper = null;
        }
        base.Dispose(disposing);
    }

    /// <summary>搜索 数据资源的基类 ，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<BaseResourceModel>> SearchAsync(
        BaseResourceSearchModel model
    ) {
        var query = session.Query<BaseResource>();
        var categoryId = model.CategoryId.GetValueOrDefault(0);
        if (categoryId > 0) {
            query = query.Where(x => x.Category.Id == categoryId);
        }
        if (model.Keywords.IsNotNullOrEmpty()) {
            query = query.Where(x => x.Name.Contains(model.Keywords) || x.Description.Contains(model.Keywords));
        }
        var total = await query.LongCountAsync();
        query = query.Select(x => new BaseResource {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Category = new Category { Id = x.Category.Id, Name = x.Category.Name },
            Tags = x.Tags,
            Roles = x.Roles,
            Creator = new AppUser { Id = x.Creator.Id, UserName = x.Creator.UserName },
            CreatedAt = x.CreatedAt,
            Updater = new AppUser { Id = x.Updater.Id, UserName = x.Updater.UserName },
            UpdatedAt = x.UpdatedAt
        });
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<BaseResourceModel> {
            Total = total,
            Data = mapper.Map<IList<BaseResourceModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }
    
    public async Task<PaginatedResponseModel<CategoryCountModel>> CountByCategoryAsync(BaseResourceStatisticRequestModel model) {
        var query = session.Query<BaseResource>();
        if (model.Type.IsNotNullOrEmpty()) {
            var type = model.Type.ToLowerInvariant();
            query = query.Where(res => res.Type == type);
        }
        var groupQuery = query.GroupBy(res => new { CategoryId = res.Category.Id, CategoryName = res.Category.Name })
            .Select(g => new { g.Key.CategoryId, g.Key.CategoryName, Count = g.Count()});
        var list = await groupQuery.ToListAsync();
        var result = new PaginatedResponseModel<CategoryCountModel> {
            Data = new List<CategoryCountModel>(list.Count)
        };
        foreach (var item in list) {
            result.Data.Add(new CategoryCountModel {
                CategoryId = item.CategoryId.ToString(),
                CategoryName = item.CategoryName,
                Count = item.Count
            });
        }
        return result;
    }

}
