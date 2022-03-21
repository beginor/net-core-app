using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories; 

/// <summary>附件表仓储实现</summary>
public partial class AppAttachmentRepository : HibernateRepository<AppAttachment, AppAttachmentModel, long>, IAppAttachmentRepository {

    public AppAttachmentRepository(ISession session, IMapper mapper) : base(session, mapper) { }

    /// <summary>附件表搜索，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
        AppAttachmentSearchModel model
    ) {
        var query = Session.Query<AppAttachment>();
        // todo: add custom query here;
        var total = await query.LongCountAsync();
        var data = await query.OrderByDescending(e => e.Id)
            .Skip(model.Skip).Take(model.Take)
            .ToListAsync();
        return new PaginatedResponseModel<AppAttachmentModel> {
            Total = total,
            Data = Mapper.Map<IList<AppAttachmentModel>>(data),
            Skip = model.Skip,
            Take = model.Take
        };
    }

}