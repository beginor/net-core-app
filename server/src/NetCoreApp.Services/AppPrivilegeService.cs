using System;
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
                    if (!string.IsNullOrEmpty(model.Module)) {
                        query = query.Where(p => p.Module == model.Module);
                    }
                    return query;
                }
            );
            var data = await repo.QueryAsync(
                query => {
                    if (!string.IsNullOrEmpty(model.Module)) {
                        query = query.Where(p => p.Module == model.Module);
                    }
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

        /// <summary>同步必须的权限</summary>
        public async Task SyncRequiredAsync(IEnumerable<string> names) {
            foreach (var name in names) {
                var exists = await Repository.ExistsAsync(name);
                if (!exists) {
                    var model = new AppPrivilege {
                        Name = name,
                        Module = name.Substring(0, name.IndexOf('.')),
                        Description = string.Empty,
                        IsRequired = true
                    };
                    await Repository.SaveAsync(model);
                }
            }
        }

        /// <summary>根据 id 删除指定的权限， 但是不能删除 IsRequired 为 true 的权限。</summary>
        public override async Task DeleteAsync(string id) {
            var entity = await Repository.GetByIdAsync(ConvertIdFromString(id));
            if (entity == null) {
                return;
            }
            if (entity.IsRequired) {
                throw new InvalidOperationException("无法删除必须的权限！");
            }
            await Repository.DeleteAsync(entity.Id);
        }

        /// <summary>返回权限表的所有模块名称</summary>
        public async Task<IList<string>> GetModulesAsync() {
            var modules = await Repository.GetModulesAsync();
            return modules;
        }

    }

}
