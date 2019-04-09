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

    public partial class UserService : IUserService {

        private UserManager<ApplicationUser> manager;

        public UserService(UserManager<ApplicationUser> manager) {
            this.manager = manager;
        }

        public async Task CreateAsync(
            ApplicationUserModel model
        ) {
            Argument.NotNull(model, nameof(model));
            var user = Mapper.Map<ApplicationUser>(model);
            var result = await manager.CreateAsync(user);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
            Mapper.Map(user, model);
        }

        public async Task DeleteAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            var user = await manager.FindByIdAsync(id);
            if (user == null) {
                return;
            }
            var result = await manager.DeleteAsync(user);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
        }

        public Task<IList<ApplicationUserModel>> GetAllAsync() {
            var users = manager.Users.ToList();
            var models = Mapper.Map<IList<ApplicationUserModel>>(users);
            return Task.FromResult(models);
        }

        public async Task<ApplicationUserModel> GetByIdAsync(string id) {
            Argument.NotNullOrEmpty(id, nameof(id));
            var user = await manager.FindByIdAsync(id);
            if (user == null) {
                return null;
            }
            return Mapper.Map<ApplicationUserModel>(user);
        }

        public async Task UpdateAsync(
            string id,
            ApplicationUserModel model
        ) {
            Argument.NotNullOrEmpty(id, nameof(id));
            Argument.NotNull(model, nameof(model));
            var user = await manager.FindByIdAsync(id);
            if (user == null) {
                throw new InvalidOperationException(
                    $"User {id} does not exists"
                );
            }
            Mapper.Map(model, user);
            var result = await manager.UpdateAsync(user);
            if (!result.Succeeded) {
                throw new InvalidOperationException(result.GetErrorsString());
            }
            Mapper.Map(user, model);
        }
    }

}
