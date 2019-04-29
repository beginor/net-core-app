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

    /// <summary>附件表服务实现</summary>
    public partial class AppAttachmentService : BaseService<IAppAttachmentRepository, AppAttachment, AppAttachmentModel, long>, IAppAttachmentService {

        public AppAttachmentService(IAppAttachmentRepository repository) : base(repository) { }

        protected override long ConvertIdFromString(string id) {
            long result;
            if (long.TryParse(id, out result)) {
                return result;
            }
            return result;
        }

        /// <summary>附件表搜索，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppAttachmentModel>> SearchAsync(
            AppAttachmentSearchModel model
        ) {
            var repo = base.Repository;
            var total = await repo.CountAsync(
                query => {
                    // todo: add custom query here;
                    return query;
                }
            );
            var data = await repo.QueryAsync(
                query => {
                    // todo: add custom query here;
                    return query.Skip(model.Skip).Take(model.Take);
                }
            );
            return new PaginatedResponseModel<AppAttachmentModel> {
                Total = total,
                Data = Mapper.Map<IList<AppAttachmentModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}
