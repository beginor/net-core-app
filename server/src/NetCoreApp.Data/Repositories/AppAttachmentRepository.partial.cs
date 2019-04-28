using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.Data.Repositories {

    partial class AppAttachmentRepository {

        public async Task<long> CountAsync(string userId, string contentType) {
            using (var session = SessionFactory.OpenSession()) {
                var query = session.Query<AppAttachment>();
                if (userId.IsNotNullOrEmpty()) {
                    query = query.Where(x => x.CreatorId == userId);
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
                    query = query.Where(x => x.CreatorId == userId);
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
