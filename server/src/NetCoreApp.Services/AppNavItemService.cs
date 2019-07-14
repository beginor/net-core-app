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
using Microsoft.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Services {

    /// <summary>导航节点（菜单）服务实现</summary>
    public partial class AppNavItemService : BaseService<IAppNavItemRepository, AppNavItem, AppNavItemModel, long>, IAppNavItemService {

        private UserManager<AppUser> userManager;

        public AppNavItemService(
            IAppNavItemRepository repository,
            UserManager<AppUser> userManager
        ) : base(repository) {
            this.userManager = userManager;
        }

        protected override long ConvertIdFromString(string id) {
            long result;
            if (long.TryParse(id, out result)) {
                return result;
            }
            return result;
        }

        /// <summary>导航节点（菜单）搜索，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<AppNavItemModel>> SearchAsync(
            AppNavItemSearchModel model
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
            return new PaginatedResponseModel<AppNavItemModel> {
                Total = total,
                Data = Mapper.Map<IList<AppNavItemModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task CreateAsync(AppNavItemModel model, string userName) {
            // Check parameters;
            Argument.NotNull(model, nameof(model));
            Argument.NotNullOrEmpty(userName, nameof(userName));
            // Map to entity;
            var entity = Mapper.Map<AppNavItem>(model);
            // Assign creator
            var user = await userManager.FindByNameAsync(userName);
            entity.Creator = user;
            entity.CreatedAt = DateTime.Now;
            // Assien updater;
            entity.Updater = user;
            entity.UpdateAt = DateTime.Now;
            // Ensure not deleted.
            entity.IsDeleted = false;
            // Save to repository;
            await Repository.SaveAsync(entity);
            // Map changes back;
            Mapper.Map(entity, model);
        }

        public async Task UpdateAsync(string id, AppNavItemModel model, string userName) {
            // Check parameters;
            Argument.NotNullOrEmpty(id, nameof(id));
            Argument.NotNull(model, nameof(model));
            Argument.NotNullOrEmpty(userName, nameof(userName));
            var entity = await Repository.GetByIdAsync(ConvertIdFromString(id));
            if (entity == null) {
                throw new InvalidOperationException(
                    $"导航节点 {id} 不存在！"
                );
            }
            // Map changes to entity;
            Mapper.Map(model, entity);
            entity.Id = ConvertIdFromString(id);
            // Assign updater;
            entity.Updater = await userManager.FindByNameAsync(userName);
            entity.UpdateAt = DateTime.Now;
            // Save to repository
            await Repository.UpdateAsync(entity);
        }

    }

}
