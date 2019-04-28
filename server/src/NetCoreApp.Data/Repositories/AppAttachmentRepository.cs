using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    public partial class AppAttachmentRepository : HibernateRepository<AppAttachment, long>, IAppAttachmentRepository {

        public AppAttachmentRepository(ISessionFactory sessionFactory) : base(sessionFactory) {

        }

    }

}
