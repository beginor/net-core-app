using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public class AppAttachmentService : BaseService<IAppAttachmentRepository, AppAttachment, AppAttachmentModel, long>, IAppAttachmentService {

        public AppAttachmentService(IAppAttachmentRepository repository) : base(repository) { }

        protected override long ConvertIdFromString(string id) {
            return long.Parse(id);
        }

        public async Task<PaginatedResponseModel<AppAttachmentModel>> Search(
            AppAttachmentSearchModel model
        ) {
            var repo = base.Repository;
            var total = await repo.QueryCountAsync(
                model.UserId,
                model.ContentType
            );
            var data = await repo.QueryAsync(
                model.UserId,
                model.ContentType,
                model.Skip,
                model.Take
            );
            return new PaginatedResponseModel<AppAttachmentModel> {
                Total = total,
                Data = Mapper.Map<IList<AppAttachmentModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task<IList<AppAttachmentModel>> GetByUser(string userId) {
            var data = await Repository.QueryAsync(a => a.UserId == userId);
            var models = Mapper.Map<IList<AppAttachmentModel>>(data);
            return models;
        }

    }

}
