using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    public interface IAppAttachmentRepository : IRepository<AppAttachment, long> {

        Task<long> CountAsync(string userId, string contentType);

        Task<IList<AppAttachment>> QueryAsync(
            string userId,
            string contentType,
            int skip,
            int take
        );

    }

}
