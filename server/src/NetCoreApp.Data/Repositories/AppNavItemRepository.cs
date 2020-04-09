using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using Dapper;
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

        public async Task SaveAsync(AppNavItemModel model, string userName) {
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
                entity.UpdatedAt = DateTime.Now;
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
                entity.UpdatedAt = DateTime.Now;
                await session.UpdateAsync(entity);
                await session.FlushAsync();
                session.Clear();
            }
        }

        public async Task<MenuNodeModel> GetMenuAsync(string[] roles) {
            if (roles == null) {
                throw new ArgumentNullException("roles can not be null.");
            }
            using var session = OpenSession();
            var conn = session.Connection;
            var sql = @"
                with recursive cte as (
                    select p.id, p.parent_id, p.title, p.tooltip, p.icon, p.url, p.sequence, p.roles, p.target
                    from public.app_nav_items p
                    where p.id = 0
                    union all
                    select c.id, c.parent_id, c.title, c.tooltip, c.icon, c.url, c.sequence, c.roles, c.target
                    from public.app_nav_items c
                    inner join cte on cte.id = c.parent_id
                    where c.is_deleted = false and c.roles && @roles::character varying[]
                )
                select * from cte;
            ";
            var navItems = await conn.QueryAsync<AppNavItem>(sql, new { roles });
            var rootNavItem = navItems.First(n => n.Id == 0);
            var model = new MenuNodeModel {
                Title = rootNavItem.Title,
                Url = rootNavItem.Url,
                Icon = rootNavItem.Icon,
                Tooltip = rootNavItem.Tooltip,
                Children = FindChildrenRecursive(0, navItems)
            };
            return model;
        }

        private MenuNodeModel[] FindChildrenRecursive(long id, IEnumerable<AppNavItem> items) {
            var childCount = items.Count(item => item.ParentId == id);
            if (childCount == 0) {
                return null;
            }
            var children = items.Where(item => item.ParentId == id)
                .OrderBy(item => item.Sequence)
                .Select(item => new MenuNodeModel {
                    Title = item.Title,
                    Url = item.Url,
                    Icon = item.Icon,
                    Tooltip = item.Tooltip,
                    Target = item.Target,
                    Children = FindChildrenRecursive(item.Id, items)
                });
            return children.ToArray();
        }

    }

}
