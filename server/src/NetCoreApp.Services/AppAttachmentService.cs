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

    }

}
