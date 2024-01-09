using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>附件表仓储实现</summary>
public partial class AppAttachmentRepository(
    ISession session,
    IMapper mapper,
    CommonOption commonOption
) : HibernateRepository<AppAttachment, AppAttachmentModel, long>(session, mapper),
    IAppAttachmentRepository {

    /// <summary>附件表搜索，返回分页结果。</summary>
    public async Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
        AppAttachmentSearchModel model
    ) {
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

    public async Task SaveAsync(
        AppAttachmentModel model,
        byte[] content,
        AppUser user,
        CancellationToken token = default
    ) {
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
                var extension = Path.GetExtension(entity.FileName);
                var filePath = await SaveContentAsync(
                    entity.Id,
                    content,
                    extension,
                    token
                );
                entity.FilePath = filePath;
                await Session.UpdateAsync(entity, token);
            }
            await Session.FlushAsync(token);
            await trans.CommitAsync(token);
            Session.Clear();
            Mapper.Map(entity, model);
        }
        catch (Exception) {
            if (trans != null) {
                await trans.RollbackAsync(token);
            }
            throw;
        }
    }

    public async Task<string> SaveContentAsync(
        long id,
        byte[] content,
        string extension,
        CancellationToken token = default
    ) {
        var today = DateTime.Today;
        var storageDir = commonOption.Storage.Directory;
        var folderPath = Path.Combine(
            storageDir,
            "app_attachments",
            today.Year.ToString("D4"),
            today.Month.ToString("D2"),
            today.Day.ToString("D2")
        );
        var dirInfo = new DirectoryInfo(folderPath);
        if (!dirInfo.Exists) {
            dirInfo.Create();
        }
        var filePath = Path.Combine(folderPath, $"{id}{extension}");
        await File.WriteAllBytesAsync(filePath, content, token);
        return filePath[(storageDir.Length + 1)..];
    }

    public async Task<byte[]> GetContentAsync(long id, CancellationToken token = default) {
        var entity = await Session.GetAsync<AppAttachment>(id, token);
        if (entity == null) {
            return Array.Empty<byte>();
        }
        var filePath = Path.Combine(
            commonOption.Storage.Directory,
            entity.FilePath
        );
        var content = await File.ReadAllBytesAsync(filePath, token);
        return content;
    }

}
