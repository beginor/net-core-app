using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>用户 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private UserManager<AppUser> userMgr;
        private RoleManager<AppRole> roleMgr;
        private AutoMapper.IMapper mapper;

        public UsersController(
            UserManager<AppUser> userMgr,
            RoleManager<AppRole> roleMgr,
            AutoMapper.IMapper mapper
        ) {
            this.userMgr = userMgr;
            this.roleMgr = roleMgr;
            this.mapper = mapper;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                userMgr = null;
                roleMgr = null;
                mapper = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>创建用户</summary>
        /// <response code="200">创建用户成功</response>
        /// <response code="400">创建用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize(Policy = "app_users.create")]
        public async Task<ActionResult<AppUserModel>> Create(
            [FromBody]AppUserModel model
        ) {
            try {
                var user = await userMgr.FindByNameAsync(model.UserName);
                if (user != null) {
                    return BadRequest($"User with {model.UserName} exists!");
                }
                user = await userMgr.FindByEmailAsync(model.Email);
                if (user != null) {
                    return BadRequest($"User with {model.Email} exists!");
                }
                user = mapper.Map<AppUser>(model);
                var result = await userMgr.CreateAsync(user);
                if (result.Succeeded) {
                    mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not create user", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除指定的用户</summary>
        /// <response code="204">删除用户成功</response>
        /// <response code="400">删除用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize(Policy = "app_users.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NoContent();
                }
                var result = await userMgr.DeleteAsync(user);
                if (result.Succeeded) {
                    return NoContent();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not delete user", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取用户列表</summary>
        /// <response code="200">获取成功并返回用户列表。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpGet("")]
        [Authorize(Policy = "app_users.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppUserModel>>> GetAll(
            [FromQuery]UserSearchRequestModel model
        ) {
            try {
                var query = userMgr.Users;
                if (model.UserName.IsNotNullOrEmpty()) {
                    query = query.Where(u => u.UserName.Contains(model.UserName));
                }
                var total = await query.LongCountAsync();
                var data = await query.ToListAsync();
                var models = mapper.Map<IList<AppUserModel>>(data);
                var result = new PaginatedResponseModel<AppUserModel> {
                    Skip = model.Skip,
                    Take = model.Take,
                    Total = total,
                    Data = models
                };
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get all user", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取指定用户</summary>
        /// <response code="404">找不到指定的用户。</response>
        /// <response code="200">获取用户成功，返回用户信息。</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize(Policy = "app_users.read")]
        public async Task<ActionResult<AppUserModel>> GetById(long id) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var model = mapper.Map<AppUserModel>(user);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get user with id {id}", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>更新指定的用户</summary>
        /// <response code="200">更新成功并返回用户信息</response>
        /// <response code="400">更新用户出错并返回用户信息</response>
        /// <response code="404">指定的用户不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize(Policy = "app_users.update")]
        public async Task<ActionResult<AppUserModel>> Update(
            [FromRoute]long id,
            [FromBody]AppUserModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                mapper.Map(model, user);
                var result = await userMgr.UpdateAsync(user);
                if (result.Succeeded) {
                    mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not update user with id {id}", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="model">密码模型</param>
        /// <response code="200">更新密码成功。</response>
        /// <response code="404">用户不存在。</response>
        /// <response code="400">重置密码出错并返回信息。</response>
        /// <response code="500">服务器内部错误。</response>
        // POST api/users/{id:long}/reset-pass
        [HttpPut("{id:long}/reset-pass")]
        [Authorize(Policy = "app_users.reset_pass")]
        public async Task<ActionResult> ResetPassword(
            [FromRoute]long id,
            [FromBody]ResetPasswordModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var token = await userMgr.GeneratePasswordResetTokenAsync(user);
                var result = await userMgr.ResetPasswordAsync(
                    user,
                    token,
                    model.Password
                );
                if (result.Succeeded) {
                    return Ok();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not reset password", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="lockEndTime">锁定期限</param>
        /// <response code="200">锁定用户成功。</response>
        /// <response code="404">用户不存在。</response>
        /// <response code="400">时间小于当前时间出错并返回信息。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpPut, Route("{id:long}/lock/{lockEndTime:datetime}")]
        [Authorize(Policy = "app_users.lock")]
        public async Task<ActionResult> LockUser(
            long id,
            DateTime lockEndTime
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                if (lockEndTime < DateTime.Now) {
                    return BadRequest($"Cannot be less than the current time");
                }
                var offset = new DateTimeOffset(lockEndTime);
                await userMgr.SetLockoutEndDateAsync(user, offset);
                return Ok();
            }
            catch (Exception ex) {
                logger.Error($"Can not Lock user s", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 解锁用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <response code="200">解锁用户成功。</response>
        /// <response code="404">用户不存在。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpPut("{id:long}/unlock")]
        [Authorize(Policy = "app_users.unlock")]
        public async Task<ActionResult> Unlock(long id) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                // await manager.SetLockoutEnabledAsync(user, false);
                await userMgr.SetLockoutEndDateAsync(
                    user,
                    DateTimeOffset.Now.AddDays(-1)
                );
                await userMgr.ResetAccessFailedCountAsync(user);
                return Ok();
            }
            catch (Exception ex) {
                logger.Error($"Can not unlock", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 读取用户角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <response code="200">读取用户角色成功。</response>
        /// <response code="404">用户不存在。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpGet("{id:long}/roles")]
        [Authorize(Policy = "app_users.read_user_roles")]
        public async Task<ActionResult<IList<AppRoleModel>>> GetUserRoles(
            long id
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var roleNames = await userMgr.GetRolesAsync(user);
                var roles = await roleMgr.Roles
                        .Where(r => roleNames.Contains(r.Name))
                        .ToListAsync();
                var model = mapper.Map<IList<AppRoleModel>>(roles);
                return model.ToList();
            }
            catch (Exception ex) {
                logger.Error($"Can not User Role", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 添加用户权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="roleName">角色名称</param>
        /// <response code="200">添加用户权限用户成功。</response>
        /// <response code="404">用户或角色不存在。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpPut("{id:long}/{roleName}")]
        [Authorize(Policy = "app_users.add_role_to_user")]
        public async Task<ActionResult> AddUserToRole(
            [FromRoute]long id,
            [FromRoute]string roleName
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var role = await roleMgr.FindByNameAsync(roleName);
                if (role == null) {
                    return NotFound();
                }
                var result = await userMgr.AddToRoleAsync(user, role.Name);
                if (result.Succeeded) {
                    return Ok();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Failed to add user permissions", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 删除用户权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="roleName">角色名称</param>
        /// <response code="204">删除用户权限成功。</response>
        /// <response code="404">用户或角色不存在。</response>
        /// <response code="400">删除用户角色出错。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpDelete("{id:long}/{roleName}")]
        [Authorize(Policy = "app_users.remove_role_from_user")]
        public async Task<ActionResult> RemoveUserFromRole(
            long id,
            string roleName
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var role = await roleMgr.FindByNameAsync(roleName);
                if (role == null) {
                    return NotFound();
                }
                var isInRole = await userMgr.IsInRoleAsync(user, role.Name);
                if (!isInRole) {
                    return NoContent();
                }
                var result = await userMgr.RemoveFromRoleAsync(user, role.Name);
                if (result.Succeeded) {
                    return NoContent();
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Failed to delete user permissions", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
