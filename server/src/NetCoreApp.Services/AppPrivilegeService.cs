using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Services;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Services {

    /// <summary>系统权限服务实现</summary>
    public partial class AppPrivilegeService : BaseService<IAppPrivilegeRepository, AppPrivilege, AppPrivilegeModel, long>, IAppPrivilegeService {

        public AppPrivilegeService(
            IAppPrivilegeRepository repository,
            IMapper mapper
        ) : base(repository, mapper) { }

        protected override long ConvertIdFromString(string id) {
            long result;
            if (long.TryParse(id, out result)) {
                return result;
            }
            return result;
        }

        /// <summary>系统权限搜索，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppPrivilegeModel>> SearchAsync(
            AppPrivilegeSearchModel model
        ) {
            var repo = base.Repository;
            var total = await repo.CountAsync(
                query => {
                    // todo: add custom query here;
                    return query;
                }
            );
            var data = await repo.QueryAsync(
                query => {
                    // todo: add custom query here;
                    return query.Skip(model.Skip).Take(model.Take);
                }
            );
            return new PaginatedResponseModel<AppPrivilegeModel> {
                Total = total,
                Data = Mapper.Map<IList<AppPrivilegeModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

    }

}
