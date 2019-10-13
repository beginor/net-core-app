using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>系统权限仓储实现</summary>
    public partial class AppPrivilegeRepository : HibernateRepository<AppPrivilege, AppPrivilegeModel, long>, IAppPrivilegeRepository {

        public AppPrivilegeRepository(ISessionFactory sessionFactory, IMapper mapper) : base(sessionFactory, mapper) { }

        public async Task<PaginatedResponseModel<AppPrivilegeModel>> SearchAsync(
            AppPrivilegeSearchModel model
        ) {
            using (var session = OpenSession()) {
                var query = session.Query<AppPrivilege>();
                if (!string.IsNullOrEmpty(model.Module)) {
                    query = query.Where(p => p.Module == model.Module);
                }
                var total = await query.LongCountAsync();
                var data = await query.OrderByDescending(e => e.Id)
                    .Skip(model.Skip).Take(model.Take)
                    .ToListAsync();
                return new PaginatedResponseModel<AppPrivilegeModel> {
                    Total = total,
                    Data = Mapper.Map<IList<AppPrivilegeModel>>(data),
                    Skip = model.Skip,
                    Take = model.Take
                };
            }
        }

        /// <summary>返回权限表的所有模块</summary>
        public async Task<IList<string>> GetModulesAsync() {
            using (var session = OpenSession()) {
                var modules = await session.Query<AppPrivilege>()
                    .Select(p => p.Module)
                    .Distinct()
                    .ToListAsync();
                return modules;
            }
        }
        
        /// <summary>同步必须的权限</summary>
        public async Task SyncRequiredAsync(IEnumerable<string> names) {
            using (var session = OpenSession()) {
                foreach (var name in names) {
                    var exists = await session.Query<AppPrivilege>()
                        .AnyAsync(e => e.Name == name);
                    if (exists) {
                        continue;
                    }
                    var entity = new AppPrivilege {
                        Name = name,
                        Module = name.Substring(0, name.IndexOf('.')),
                        Description = string.Empty,
                        IsRequired = true
                    };
                    await session.SaveAsync(entity);
                }
            }
        }

        public override async Task DeleteAsync(
            long id,
            CancellationToken token = new CancellationToken()
        ) {
            using (var session = OpenSession()) {
                var entity = await session.LoadAsync<AppPrivilege>(id, token);
                if (entity == null) {
                    return;
                }
                if (entity.IsRequired) {
                    throw new InvalidOperationException("无法删除必须的权限！");
                }
                await session.DeleteAsync(entity, token);
                await session.FlushAsync(token);
                session.Clear();
            }
        }

    }

}
