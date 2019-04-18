using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    public interface IAttachmentRepository : IRepository<Attachment, long> {

        Task<IList<Attachment>> GetByUser(string userId);

    }

}
