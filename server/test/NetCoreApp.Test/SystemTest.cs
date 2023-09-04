using System;
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

namespace Beginor.NetCoreApp.Test;

[TestFixture]
public class SystemTest : BaseTest {

    [Test]
    public async Task _01_CanInitSystem() {
        await CreatePrivilegesAsync();
        var adminRoleName = "administrators";
        await CreateAdminRoleAsync(adminRoleName, "系统管理员");
        var anonymousRoleName = "anonymous";
        await CreateAnonymousRoleAsync(anonymousRoleName, "匿名用户");
        var adminUserName = "admin";
        var adminUserId = await CreateAdminUserAsync(adminUserName, "1qaz@WSX", adminRoleName);
        await CreateSystemMenuAsync(adminUserName, adminRoleName, anonymousRoleName);
    }

    [Test]
    public async Task _02_CanResetPassword() {
        var manager = ServiceProvider.GetService<UserManager<AppUser>>();
        var username = "admin";
        var password = "1qaz@WSX";
        var user = await manager.FindByNameAsync("admin");
        if (user == null) {
            Assert.Fail($"User {username} does not exits!");
            return;
        }
        var token = await manager.GeneratePasswordResetTokenAsync(user);
        Console.WriteLine($"Reset token is ${token} ");
        var result = await manager.ResetPasswordAsync(user, token, password);
        Assert.IsTrue(result.Succeeded);
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

    private async Task CreatePrivilegesAsync() {
        var repo = ServiceProvider.GetService<IAppPrivilegeRepository>();
        var count = await repo.CountAllAsync();
        if (count > 0) {
            return;
        }
        var privileges = new AppPrivilegeModel[] {
            new () { Module = "菜单", Name = "app_nav_items.create", Description = "创建菜单项", IsRequired = true },
            new () { Module = "菜单", Name = "app_nav_items.delete", Description = "删除菜单项", IsRequired = true },
            new () { Module = "菜单", Name = "app_nav_items.update", Description = "更新菜单项", IsRequired = true },
            new () { Module = "菜单", Name = "app_nav_items.read", Description = "读取菜单项列表", IsRequired = true },
            new () { Module = "菜单", Name = "app_nav_items.read_by_id", Description = "读取菜单项详情", IsRequired = true },
            new () { Module = "用户", Name = "app_users.create", Description = "创建用户", IsRequired = true },
            new () { Module = "用户", Name = "app_users.delete", Description = "删除用户", IsRequired = true },
            new () { Module = "用户", Name = "app_users.update", Description = "更新用户", IsRequired = true },
            new () { Module = "用户", Name = "app_users.reset_pass", Description = "重置用户密码", IsRequired = true },
            new () { Module = "用户", Name = "app_users.lock", Description = "锁定用户", IsRequired = true },
            new () { Module = "用户", Name = "app_users.unlock", Description = "锁定用户", IsRequired = true },
            new () { Module = "用户", Name = "app_users.add_role_to_user", Description = "添加用户角色", IsRequired = true },
            new () { Module = "用户", Name = "app_users.remove_role_from_user", Description = "删除用户的角色", IsRequired = true },
            new () { Module = "用户", Name = "app_users.read_by_id", Description = "读取用户信息详情", IsRequired = true },
            new () { Module = "用户", Name = "app_users.read_user_roles", Description = "读取用户角色", IsRequired = true },
            new () { Module = "用户", Name = "app_users.read", Description = "读取用户列表", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.create", Description = "创建角色", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.remove_privilige_from_role", Description = "删除角色的权限", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.delete", Description = "删除角色", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.update", Description = "更新角色", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.add_privilege_to_role", Description = "添加角色的权限", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.read_by_id", Description = "读取角色详情", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.read_role_privileges", Description = "读取角色的权限", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.read_users_in_role", Description = "读取角色的用户列表", IsRequired = true },
            new () { Module = "角色", Name = "app_roles.read", Description = "读取角色列表", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.sync_required", Description = "同步必须的权限", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.update", Description = "更新权限", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.create", Description = "创建权限", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.delete", Description = "删除权限", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.read_by_id", Description = "读取权限详情", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.read_modules", Description = "读取权限的模块列表", IsRequired = true },
            new () { Module = "权限", Name = "app_privileges.read", Description = "读取权限信息列表", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.read", Description = "读取存储配置列表", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.create", Description = "创建存储配置", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.delete", Description = "删除存储配置", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.read_by_id", Description = "读取存储配置详情", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.update", Description = "更新存储配置", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.read_folder_content", Description = "读取存储配置的文件列表", IsRequired = true },
            new () { Module = "存储配置", Name = "app_storages.read_file_content", Description = "读取存储配置的文件内容", IsRequired = true },
            new () { Module = "审计日志", Name = "app_audit_logs.create", Description = "创建审计日志", IsRequired = true },
            new () { Module = "审计日志", Name = "app_audit_logs.read_by_id", Description = "读取审计日志详情", IsRequired = true },
            new () { Module = "审计日志", Name = "app_audit_logs.read", Description = "读取审计日志列表", IsRequired = true },
            new () { Module = "审计日志", Name = "app_audit_logs.read_stat", Description = "读取统计数据", IsRequired = true },
            new () { Module = "运行日志", Name = "app_logs.read_by_id", Description = "读取运行日志详情", IsRequired = true },
            new () { Module = "运行日志", Name = "app_logs.read", Description = "读取运行日志列表", IsRequired = true },
            new () { Module = "附件", Name = "app_attachments.update", Description = "更新附件", IsRequired = true },
            new () { Module = "附件", Name = "app_attachments.delete", Description = "删除附件", IsRequired = true },
            new () { Module = "附件", Name = "app_attachments.create", Description = "创建附件", IsRequired = true },
            new () { Module = "附件", Name = "app_attachments.read_by_id", Description = "读取附件详情", IsRequired = true },
            new () { Module = "附件", Name = "app_attachments.read", Description = "读取附件列表", IsRequired = true },
            new () { Module = "JSON数据", Name = "app_json_data.update", Description = "更新JSON数据", IsRequired = true },
            new () { Module = "JSON数据", Name = "app_json_data.delete", Description = "删除JSON数据", IsRequired = true },
            new () { Module = "JSON数据", Name = "app_json_data.read", Description = "读取JSON数据列表", IsRequired = true },
            new () { Module = "JSON数据", Name = "app_json_data.read_by_id", Description = "读取JSON数据详情", IsRequired = true },
            new () { Module = "客户端错误", Name = "app_client_errors.create", Description = "创建客户端错误记录", IsRequired = true },
            new () { Module = "客户端错误", Name = "app_client_errors.read", Description = "读取客户端错误记录列表", IsRequired = true },
            new () { Module = "客户端错误", Name = "app_client_errors.read_by_id", Description = "读取客户端错误记录详情", IsRequired = true },
            new () { Module = "帐户", Name = "account.get_info_by_token", Description = "帐户通过 token 获取帐户信息", IsRequired = true },
        };
        foreach (var privilege in privileges) {
            await repo.SaveAsync(privilege);
            Assert.IsNotEmpty(privilege.Id);
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
            Icon = "bi/app-indicator",
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
            Icon = "bi/compass",
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
            Icon = "bi/person",
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
            Icon = "bi/shield-lock",
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
            Icon = "bi/shield-shaded",
            Url = "/admin/privileges",
            Sequence = 4,
            Roles = new [] { adminRoleName }
        };
        await repo.SaveAsync(privilegesManageItem, userName);
        Assert.IsNotEmpty(privilegesManageItem.Id);
        // 存储管理
        var storageManageItem = new AppNavItemModel {
            ParentId = adminItem.Id,
            Title = "存储管理",
            Icon = "bi/folder2-open",
            Url = "/admin/storages",
            Sequence = 5,
            Roles = new [] { adminRoleName }
        };
        await repo.SaveAsync(storageManageItem);
        Assert.IsNotEmpty(storageManageItem.Id);
        // 审计日志
        var auditLogsItem = new AppNavItemModel {
            ParentId = adminItem.Id,
            Title = "审计日志",
            Icon = "bi/list-check",
            Url = "/admin/audit-logs",
            Sequence = 6,
            Roles = new [] { adminRoleName }
        };
        await repo.SaveAsync(auditLogsItem, userName);
        Assert.IsNotEmpty(auditLogsItem.Id);
        // 运行日志
        var logsItem = new AppNavItemModel {
            ParentId = adminItem.Id,
            Title = "运行日志",
            Icon = "bi/exclamation-diamond",
            Url = "/admin/logs",
            Sequence = 7,
            Roles = new [] { adminRoleName }
        };
        await repo.SaveAsync(logsItem, userName);
        Assert.IsNotEmpty(logsItem.Id);
    }
}
