using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Api.Controllers;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Test {

    [TestFixture]
    public class SystemTest : BaseTest {

        [Test]
        public async Task _01_CanInitSystem() {
            var privilegeCount = await SyncSyncRequiredPrivilegesAsync();
            Assert.Greater(privilegeCount, 0);
            var adminRoleName = "administrators";
            await CreateAdminRoleAsync(adminRoleName, "系统管理员");
            var anonymousRoleName = "anonymous";
            await CreateAnonymousRoleAsync(anonymousRoleName, "匿名用户");
            var adminUserName = "admin";
            var adminUserId = await CreateAdminUserAsync(adminUserName, "1qaz@WSX", adminRoleName);
            await CreateSystemMenuAsync(adminUserName, adminRoleName, anonymousRoleName);
        }

        private async Task<long> SyncSyncRequiredPrivilegesAsync() {
            var controller = ServiceProvider.GetService<AppPrivilegeController>();
            await controller.SyncRequired();
            var repo = ServiceProvider.GetService<IAppPrivilegeRepository>();
            return await repo.CountAllAsync();
        }

        private async Task CreateAdminRoleAsync(string roleName, string description) {
            var manager = ServiceProvider.GetService<RoleManager<AppRole>>();
            var roleExists = await manager.RoleExistsAsync(roleName);
            if (!roleExists) {
                // create administrators role;
                var role = new AppRole {
                    Name = roleName,
                    Description = description
                };
                await manager.CreateAsync(role);
                Assert.IsNotEmpty(role.Id);
                // create privileges;
                var repo = ServiceProvider.GetService<IAppPrivilegeRepository>();
                var privileges = await repo.GetAllAsync();
                foreach (var p in privileges) {
                    var claim = new Claim(Consts.PrivilegeClaimType, p.Name);
                    await manager.AddClaimAsync(role, claim);
                }
                var claims = await manager.GetClaimsAsync(role);
                Assert.AreEqual(privileges.Count, claims.Count);
            }
        }

        private async Task CreateAnonymousRoleAsync(string roleName, string description) {
            var manager = ServiceProvider.GetService<RoleManager<AppRole>>();
            var roleExists = await manager.RoleExistsAsync(roleName);
            if (!roleExists) {
                var role = new AppRole {
                    Name = roleName,
                    Description = description,
                    IsAnonymous = true
                };
                await manager.CreateAsync(role);
                Assert.IsNotEmpty(role.Id);
            }
        }

        private async Task<string> CreateAdminUserAsync(string username, string password, string roleName) {
            var manager = ServiceProvider.GetService<UserManager<AppUser>>();
            var user = await manager.FindByNameAsync(username);
            if (user == null) {
                // create admin user;
                user = new AppUser {
                    UserName = username,
                    Email = "admin@local.com",
                    EmailConfirmed = true,
                    PhoneNumber = "02088888888",
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false
                };
                await manager.CreateAsync(user);
                Assert.IsNotNull(user.Id);
                // add password;
                var result = await manager.AddPasswordAsync(user, password);
                Assert.IsTrue(result.Succeeded);
                // add to administrators;
                if (!await manager.IsInRoleAsync(user, roleName)) {
                    var result2 = await manager.AddToRoleAsync(user, roleName);
                    Assert.IsTrue(result2.Succeeded);
                }
            }
            return user.Id;
        }

        private async Task CreateSystemMenuAsync(string userName, string adminRoleName, string anonymousRoleName) {
            var repo = ServiceProvider.GetService<IAppNavItemRepository>();
            if (await repo.CountAllAsync() > 0) {
                return;
            }
            // 根应用
            var rootNavItem = new AppNavItemModel {
                Title = ".NET Core App",
                Icon = "fab fa-angular",
                Url = "/",
                Sequence = 0,
                Roles = new [] { adminRoleName, anonymousRoleName }
            };
            await repo.SaveAsync(rootNavItem, userName);
            Assert.IsNotEmpty(rootNavItem.Id);
            // 首页
            var homeItem = new AppNavItemModel {
                ParentId = rootNavItem.Id,
                Title = "首页",
                Url = "/home",
                Sequence = 1,
                Roles = new [] { adminRoleName, anonymousRoleName }
            };
            await repo.SaveAsync(homeItem, userName);
            Assert.IsNotEmpty(homeItem.Id);
            // 管理
            var adminItem = new AppNavItemModel {
                ParentId = rootNavItem.Id,
                Title = "管理",
                Url = "/admin",
                Sequence = 2,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(adminItem, userName);
            Assert.IsNotEmpty(adminItem.Id);
            // 关于
            var aboutItem = new AppNavItemModel {
                ParentId = rootNavItem.Id,
                Title = "关于",
                Url = "/about",
                Sequence = 3,
                Roles = new [] { adminRoleName, anonymousRoleName }
            };
            await repo.SaveAsync(aboutItem, userName);
            Assert.IsNotEmpty(aboutItem.Id);
            // 导航管理
            var menuManageItem = new AppNavItemModel {
                ParentId = adminItem.Id,
                Title = "导航菜单",
                Icon = "fas fa-compass",
                Url = "/admin/nav-items",
                Sequence = 1,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(menuManageItem, userName);
            Assert.IsNotEmpty(menuManageItem.Id);
            // 用户管理
            var userManageItem = new AppNavItemModel {
                ParentId = adminItem.Id,
                Title = "用户管理",
                Icon = "fas fa-users",
                Url = "/admin/users",
                Sequence = 2,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(userManageItem, userName);
            Assert.IsNotEmpty(userManageItem.Id);
            // 角色管理
            var roleManageItem = new AppNavItemModel {
                ParentId = adminItem.Id,
                Title = "角色管理",
                Icon = "fas fa-user-tag",
                Url = "/admin/roles",
                Sequence = 3,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(roleManageItem, userName);
            Assert.IsNotEmpty(roleManageItem.Id);
            // 权限管理
            var privilegesManageItem = new AppNavItemModel {
                ParentId = adminItem.Id,
                Title = "权限管理",
                Icon = "fas fa-user-shield",
                Url = "/admin/privileges",
                Sequence = 4,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(privilegesManageItem, userName);
            Assert.IsNotEmpty(privilegesManageItem.Id);
            // 权限管理
            var auditLogsItem = new AppNavItemModel {
                ParentId = adminItem.Id,
                Title = "审计日志",
                Icon = "fas fa-list",
                Url = "/admin/audit-logs",
                Sequence = 5,
                Roles = new [] { adminRoleName }
            };
            await repo.SaveAsync(auditLogsItem, userName);
            Assert.IsNotEmpty(auditLogsItem.Id);
        }
    }

}
