using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    public partial interface IAppClientErrorRepository : IRepository<AppClientErrorModel, long> {

        Task<PaginatedResponseModel<AppClientErrorModel>> SearchAsync(
            AppClientErrorSearchModel model
        );

    }

}
