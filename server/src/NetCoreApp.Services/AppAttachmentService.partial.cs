using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    partial class AppAttachmentService {

//        public async Task<PaginatedResponseModel<AppAttachmentModel>> Search(
//            AppAttachmentSearchModel model
//        ) {
//            var repo = base.Repository;
//            var total = await repo.CountAsync(
//                query => {
//                    // add custom query here;
//                    return query;
//                }
//            );
//            var data = await repo.QueryAsync(
//                query => {
//                    // add custom query here;
//                    return query.Skip(model.Skip).Take(model.Take);
//                }
//            );
//            return new PaginatedResponseModel<AppAttachmentModel> {
//                Total = total,
//                Data = Mapper.Map<IList<AppAttachmentModel>>(data),
//                Skip = model.Skip,
//                Take = model.Take
//            };
//        }

        public async Task<IList<AppAttachmentModel>> GetByUser(string userId) {
            var data = await Repository.QueryAsync(a => a.CreatorId == userId);
            var models = Mapper.Map<IList<AppAttachmentModel>>(data);
            return models;
        }

    }

}
