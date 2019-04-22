using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    public interface IAppAttachmentRepository : IRepository<AppAttachment, long> {

        Task<IList<AppAttachment>> GetByUser(string userId);

    }

}
