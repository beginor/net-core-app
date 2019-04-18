using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    public class AttachmentService : BaseService<IAttachmentRepository, Attachment, AttachmentModel, long>, IAttachmentService {

        public AttachmentService(IAttachmentRepository repository) : base(repository) { }

        protected override long ConvertIdFromString(string id) {
            return long.Parse(id);
        }

    }

}
