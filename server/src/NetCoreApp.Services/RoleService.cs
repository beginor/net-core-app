using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Identity;
using NHibernate.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Services {

    public partial class RoleService : IRoleService {

        private RoleManager<ApplicationRole> manager;

        public RoleService(RoleManager<ApplicationRole> manager) {
            this.manager = manager;
        }

        public async Task CreateAsync(
            ApplicationRoleModel model
        ) {
            Argument.NotNull(model, nameof(model));
            var role = Mapper.Map<ApplicationRole>(model);
            if (await manager.RoleExistsAsync(role.Name)) {
                throw new InvalidOperationException($"Role {role.Name} exists!");
            }
            var result = await manager.CreateAsync(role);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
            Mapper.Map(role, model);
        }

        public async Task DeleteAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            var role = await manager.FindByIdAsync(id);
            if (role == null) {
                return;
            }
            var result = await manager.DeleteAsync(role);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
        }

        public Task<IList<ApplicationRoleModel>> GetAllAsync() {
            var roles = manager.Roles.ToList();
            var models = Mapper.Map<IList<ApplicationRoleModel>>(roles);
            return Task.FromResult(models);
        }

        public async Task<ApplicationRoleModel> GetByIdAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            var role = await manager.FindByIdAsync(id);
            if (role == null) {
                return null;
            }
            return Mapper.Map<ApplicationRoleModel>(role);
        }

        public async Task UpdateAsync(
            string id,
            ApplicationRoleModel model
        ) {
            Argument.NotNullOrEmpty(id, nameof(id));
            Argument.NotNull(model, nameof(model));
            var role = await manager.FindByIdAsync(id);
            if (role == null) {
                throw new InvalidOperationException(
                    $"Role {id} does not exists"
                );
            }
            Mapper.Map(model, role);
            var result = await manager.UpdateAsync(role);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
            Mapper.Map(role, model);
        }

    }

}
