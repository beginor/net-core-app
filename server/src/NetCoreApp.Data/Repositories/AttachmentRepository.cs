using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using Dapper;
using NHibernate;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Data.Repositories {

    public class AttachmentRepository
        : HibernateRepository<Attachment, long>, IAttachmentRepository {

        public AttachmentRepository(ISessionFactory sessionFactory)
            : base(sessionFactory) {
        }

        public async Task<IList<Attachment>> GetByUser(string userId) {
            Argument.NotNullOrEmpty(userId, nameof(userId));
            var data = await base.QueryAsync(a => a.UserId == userId);
            return data;

            //using (var session = SessionFactory.OpenSession()) {
            //    var query = session.Query<Attachment>().Where(a => a.UserId == userId);
            //    var data = await query.ToListAsync();
            //    return data;
            //}

            //using (var session = SessionFactory.OpenSession()) {
            //    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            //    var conn = session.Connection;
            //    var data = await conn.QueryAsync<Attachment>("select * from public.attachments where user_id = #userId", new { userId });
            //    return data.ToList();
            //}
        }

    }

}
