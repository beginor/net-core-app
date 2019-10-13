using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>导航节点（菜单）仓储实现</summary>
    public partial class AppNavItemRepository : HibernateRepository<AppNavItem, AppNavItemModel, long>, IAppNavItemRepository {

        private UserManager<AppUser> userManager;

        public AppNavItemRepository(
            ISessionFactory sessionFactory,
            IMapper mapper,
            UserManager<AppUser> userManager
        ) : base(sessionFactory, mapper) {
            this.userManager = userManager;
        }

        protected override void Dispose(
            bool disposing
        ) {
            base.Dispose(disposing);
            if (disposing) {
                userManager = null;
            }
        }

        public override async Task DeleteAsync(
            long id,
            CancellationToken token = default(CancellationToken)
        ) {
            using (var session = SessionFactory.OpenSession()) {
                var entity = await session.GetAsync<AppNavItem>(id);
                if (entity == null) {
                    return;
                }
                entity.IsDeleted = true;
                await session.UpdateAsync(entity);
                await session.FlushAsync();
                session.Clear();
            }
        }

        public async Task<PaginatedResponseModel<AppNavItemModel>> SearchAsync(
            AppNavItemSearchModel model
        ) {
            using (var session = OpenSession()) {
                var query = session.Query<AppNavItem>();
                // todo: add custom query here;
                var total = await query.LongCountAsync();
                var data = await query.OrderByDescending(e => e.Id)
                    .Skip(model.Skip).Take(model.Take)
                    .ToListAsync();
                return new PaginatedResponseModel<AppNavItemModel> {
                    Total = total,
                    Data = Mapper.Map<IList<AppNavItemModel>>(data),
                    Skip = model.Skip,
                    Take = model.Take
                };
            }
        }
        
        public async Task CreateAsync(AppNavItemModel model, string userName) {
            // Check parameters;
            Argument.NotNull(model, nameof(model));
            Argument.NotNullOrEmpty(userName, nameof(userName));
            // Map to entity;
            var entity = Mapper.Map<AppNavItem>(model);
            using (var session = OpenSession()) {
                var user = await session.Query<AppUser>()
                    .FirstOrDefaultAsync(
                        u => u.UserName == userName
                    );
                // Assign creator;
                entity.Creator = user;
                entity.CreatedAt = DateTime.Now;
                // Assign updater;
                entity.Updater = user;
                entity.UpdateAt = DateTime.Now;
                // Ensure not deleted.
                entity.IsDeleted = false;
                await session.SaveAsync(entity);
                await session.FlushAsync();
                session.Clear();
            }
            Mapper.Map(entity, model);
        }

        public async Task UpdateAsync(
            long id,
            AppNavItemModel model,
            string userName
        ) {
            // Check parameters;
            if (id <= 0) {
                throw new ArgumentOutOfRangeException(nameof(id), "NavItemId <= 0 !");
            }
            Argument.NotNull(model, nameof(model));
            Argument.NotNullOrEmpty(userName, nameof(userName));
            using (var session = OpenSession()) {
                var entity = await session.LoadAsync<AppNavItem>(id);
                if (entity == null) {
                    throw new InvalidOperationException(
                        $"导航节点 {id} 不存在！"
                    );
                }
                Mapper.Map(model, entity);
                entity.Id = id;
                entity.Updater = await session.Query<AppUser>()
                    .FirstAsync(u => u.UserName == userName);
                entity.UpdateAt = DateTime.Now;
                await session.UpdateAsync(entity);
                await session.FlushAsync();
                session.Clear();
            }
        }

    }

}
