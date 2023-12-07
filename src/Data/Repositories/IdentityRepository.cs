using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Dapper;
using NHibernate;

namespace Beginor.NetCoreApp.Data.Repositories;

public partial class IdentityRepository : Disposable, IIdentityRepository {

    private ISessionFactory factory;

    public IdentityRepository(ISessionFactory factory) {
        this.factory = factory;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here;
        }
    }

    public async Task<IList<AppRoleModel>> SearchAsync(AppRoleSearchModel model) {
        var unitCode = model.OrganizeUnitCode;
        using (var session = factory.OpenSession()) {
            var conn = session.Connection;
            var sql = @"
                select ar.id, ir.name, ar.description, ar.is_default, ar.is_anonymous,
                    (select count(*)
                    from public.aspnet_user_roles aur
                    inner join public.app_users au on au.id = aur.user_id
                    inner join public.app_organize_units aou on aou.id = au.organize_unit_id
                        and aou.is_deleted = false
                    where aur.role_id = ar.id and aou.code like concat(@unitCode, '%')
                    ) as user_count
                from public.aspnet_roles ir
                inner join public.app_roles ar on ar.id = ir.id
                left join public.aspnet_user_roles ur on ir.id = ur.role_id
                group by ar.id, ir.name, ar.description, ar.is_default
                order by ar.id;";
            var data = await conn.QueryAsync<AppRoleModel>(sql, new { unitCode });
            return data.ToList();
        }
    }

}
