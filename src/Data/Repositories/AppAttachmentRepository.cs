using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;

using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

/// <summary>附件表仓储实现</summary>
public partial class AppAttachmentRepository(
    ISession session,
    IMapper mapper,
    IHostEnvironment env,
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
            .Select(x => new AppAttachment {
                Id = x.Id,
                BusinessId = x.BusinessId,
                FileName = x.FileName,
                Length = x.Length,
                FilePath = x.FilePath,
                ContentType = x.ContentType,
                Creator = new AppUser {
                    Id = x.Creator.Id,
                    UserName = x.Creator.UserName
                },
                CreatedAt = x.CreatedAt,
            })
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
        FileInfo file,
        ClaimsPrincipal user,
        Thumbnail? thumbnail = null,
        CancellationToken token = default
    ) {
        var entity = Mapper.Map<AppAttachment>(model);
        var isLocalTrans = false;
        var trans = Session.GetCurrentTransaction();
        if (trans == null) {
            trans = Session.BeginTransaction();
            isLocalTrans = true;
        }
        try {
            entity.Creator = await Session.GetAsync<AppUser>(user.GetUserId(), token);
            entity.CreatedAt = DateTime.Now;
            await Session.SaveAsync(entity, token);
            await Session.FlushAsync(token);
            thumbnail ??= FileHelper.GetThumbnail(file.FullName, env);
            await SaveThumbnailAsync(entity.Id, thumbnail.Content, token);
            var today = DateTime.Today;
            var storageDir = GetAttachmentStorageDirectory();
            var todayFolder = Path.Combine(
                $"{today.Year:D4}",
                $"{today.Month:D2}",
                $"{today.Day:D2}"
            );
            var attachmentFolder = Path.Combine(storageDir, todayFolder);
            if (!Directory.Exists(attachmentFolder)) {
                Directory.CreateDirectory(attachmentFolder);
            }
            var filePath = Path.Combine(todayFolder, $"{entity.Id}{file.Extension}");
            var destFolder = Path.Combine(storageDir, filePath);
            file.MoveTo(destFolder);
            entity.FilePath = filePath;
            await Session.FlushAsync(token);
            if (isLocalTrans) {
                await trans.CommitAsync(token);
            }
            Session.Clear();
            Mapper.Map(entity, model);
        }
        catch (Exception) {
            if (isLocalTrans) {
                await trans.RollbackAsync(token);
            }
            throw;
        }
    }

    private async Task SaveThumbnailAsync(
        long id,
        byte[] content,
        CancellationToken token = default
    ) {
        var query = Session.CreateSQLQuery(
            "update public.app_attachments set content = :content where id = :id"
        );
        query.SetBinary("content", content);
        query.SetInt64("id", id);
        await query.ExecuteUpdateAsync(token);
    }

    public async Task<byte[]> GetThumbnailAsync(long id, CancellationToken token = default) {
        var query = Session.CreateSQLQuery(
            "select content from public.app_attachments where id = :id"
        );
        query.AddScalar("content", NHibernateUtil.Binary);
        query.SetInt64("id", id);
        var content = await query.UniqueResultAsync<byte[]>(token);
        return content ?? Array.Empty<byte>();
    }

    public string GetAttachmentTempDirectory(string userId) {
        var userTempFolder = Path.Combine(
            env.ContentRootPath,
            commonOption.Storage.Directory,
            commonOption.Storage.TempDirectory,
            userId
        );
        if (!Directory.Exists(userTempFolder)) {
            Directory.CreateDirectory(userTempFolder);
        }
        return userTempFolder;
    }

    public string GetAttachmentStorageDirectory() {
        var folder = Path.Combine(
            env.ContentRootPath,
            commonOption.Storage.Directory,
            "app_attachments"
        );
        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }
        return folder;
    }

    public async Task<int> DeleteByBusinessIdAsync(long businessId, CancellationToken token = default) {
        var sqlQuery = Session.CreateSQLQuery(
            "delete from public.app_attachments where business_id = :businessId"
        );
        sqlQuery.SetInt64("businessId", businessId);
        var deleted = await sqlQuery.ExecuteUpdateAsync(token);
        return deleted;
    }

}
