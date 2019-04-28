using System.Collections.Generic;
using System.Linq;
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
            long result;
            if (long.TryParse(id, out result)) {
                return result;
            }
            return result;
        }

        public async Task<PaginatedResponseModel<AppAttachmentModel>> Search(
            AppAttachmentSearchModel model
        ) {
            var repo = base.Repository;
            var total = await repo.CountAsync(
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
            var data = await Repository.QueryAsync(a => a.CreatorId == userId);
            var models = Mapper.Map<IList<AppAttachmentModel>>(data);
            return models;
        }

    }

}
