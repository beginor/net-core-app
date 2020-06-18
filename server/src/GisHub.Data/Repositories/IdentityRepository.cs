using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Dapper;
using NHibernate;

namespace Beginor.GisHub.Data.Repositories {

    public partial class IdentityRepository : Disposable, IIdentityRepository {

        private ISessionFactory factory;

        public IdentityRepository(ISessionFactory factory) {
            this.factory = factory;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                factory = null;
            }
        }

        public async Task<IList<AppRoleModel>> SearchAsync(AppRoleSearchModel model) {
            using (var session = factory.OpenSession()) {
                var conn = session.Connection;
                var sql = new StringBuilder();
                sql.AppendLine("select ar.id, ir.name, ar.description, ar.is_default, ar.is_anonymous, count(ur.*) as user_count");
                sql.AppendLine("from public.aspnet_roles ir");
                sql.AppendLine("inner join public.app_roles ar on ar.id = ir.id");
                sql.AppendLine("left join public.aspnet_user_roles ur on ir.id = ur.role_id");
                sql.AppendLine("group by ar.id, ir.name, ar.description, ar.is_default");
                sql.AppendLine("order by ar.id;");
                var data = await conn.QueryAsync<AppRoleModel>(sql.ToString());
                return data.ToList();
            }
        }

    }

}
