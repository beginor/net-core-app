using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    partial interface IAppAttachmentRepository {

        Task<long> CountAsync(string userId, string contentType);

        Task<IList<AppAttachment>> QueryAsync(
            string userId,
            string contentType,
            int skip,
            int take
        );

    }

}
