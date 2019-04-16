using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using NHibernate;

namespace Beginor.NetCoreApp.Data.Repositories {

    public class AttachmentRepository
        : HibernateRepository<Attachment, long>, IAttachmentRepository {

        public AttachmentRepository(ISessionFactory sessionFactory)
            : base(sessionFactory) {
        }

    }

}
