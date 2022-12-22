using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories;

public partial interface IIdentityRepository {

    Task<IList<AppRoleModel>> SearchAsync(
        AppRoleSearchModel model
    );

}
