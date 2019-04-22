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

    }

}
