using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
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
    public async Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(AppAttachmentSearchModel model) {
        var query = Session.Query<AppAttachment>();
        var businessId = model.BusinessId.GetValueOrDefault(0);
        if (businessId > 0) {
            query = query.Where(e => e.BusinessId == businessId);
        }
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

    public async Task SaveAsync(AppAttachmentModel model, byte[] content, AppUser user, CancellationToken token = default) {
        var entity = Mapper.Map<AppAttachment>(model);
        entity.Creator = user;
        entity.CreatedAt = DateTime.Now;
        ITransaction? trans = null;
        try {
            trans = Session.BeginTransaction();
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            Session.Clear();
            if (content.Length > 0) {
                await SaveContentAsync(entity.Id, content, token);
            }
            await trans.CommitAsync(token);
            Mapper.Map(entity, model);
        }
        catch (Exception) {
            if (trans != null) {
                await trans.RollbackAsync(token);
            }
            throw;
        }
    }

    public async Task SaveContentAsync(long id, byte[] content, CancellationToken token = default) {
        var sql = @"update public.app_attachments
                    set content = @content
                    where id = @id";
        var conn = Session.Connection;
        await conn.ExecuteAsync(sql, new { id, content });
    }

    public async Task<byte[]> GetContentAsync(long id, CancellationToken token = default) {
        var sql = @"select content from public.app_attachments where id = @id";
        var conn = Session.Connection;
        var content = await conn.ExecuteScalarAsync<byte[]>(sql, new { id });
        return content;
    }

}
