using System.Collections.Generic;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Data.Repositories {

    public partial interface IIdentityRepository {

        Task<IList<AppRoleModel>> SearchAsync(
            AppRoleSearchModel model
        );

    }

}
