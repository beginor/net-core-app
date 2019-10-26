using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using NHibernate.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Data.Repositories {

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
                // var query = from role in session.Query<AppRole>()
                //     join userRole in session.Query<IdentityUserRole>()
                //     on role.Id equals userRole.RoleId into g
                //     select new AppRoleModel {
                //         Id = role.Id,
                //         Name = role.Name,
                //         Description = role.Description,
                //         IsDefault = role.IsDefault,
                //         UserCount = g.Count()
                //     };
                var conn = session.Connection;
                var sql = new StringBuilder();
                sql.AppendLine("select ar.id, ir.name, ar.description, ar.is_default, count(ur.*) as user_count");
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
