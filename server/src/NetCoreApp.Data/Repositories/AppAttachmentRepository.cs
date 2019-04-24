using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using Dapper;
using NHibernate;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Data.Repositories {

    public class AppAttachmentRepository
        : HibernateRepository<AppAttachment, long>, IAppAttachmentRepository {

        public AppAttachmentRepository(ISessionFactory sessionFactory)
            : base(sessionFactory) {
        }

        public async Task<long> QueryCountAsync(string userId, string contentType) {
            using (var session = SessionFactory.OpenSession()) {
                var query = session.Query<AppAttachment>();
                if (userId.IsNotNullOrEmpty()) {
                    query = query.Where(x => x.UserId == userId);
                }
                if (contentType.IsNotNullOrEmpty()) {
                    query = query.Where(x => x.ContentType == contentType);
                }
                var count = await query.LongCountAsync();
                return count;
            }
        }

        public async Task<IList<AppAttachment>> QueryAsync(
            string userId,
            string contentType,
            int skip,
            int take
        ) {
            using (var session = SessionFactory.OpenSession()) {
                var query = session.Query<AppAttachment>();
                if (userId.IsNotNullOrEmpty()) {
                    query = query.Where(x => x.UserId == userId);
                }
                if (contentType.IsNotNullOrEmpty()) {
                    query = query.Where(x => x.ContentType == contentType);
                }
                var data = await query.Skip(skip).Take(take).ToListAsync();
                return data;
            }
        }



    }

}
