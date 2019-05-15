using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
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

        public UsersController(
            UserManager<AppUser> userMgr,
            RoleManager<AppRole> roleMgr
        ) {
            this.userMgr = userMgr;
            this.roleMgr = roleMgr;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                userMgr = null;
                roleMgr = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>创建用户</summary>
        /// <response code="200">创建用户成功</response>
        /// <response code="400">创建用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
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
                user = Mapper.Map<AppUser>(model);
                var result = await userMgr.CreateAsync(user);
                if (result.Succeeded) {
                    Mapper.Map(user, model);
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
        public async Task<ActionResult> Delete(string id) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
                var models = Mapper.Map<IList<AppUserModel>>(data);
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
        public async Task<ActionResult<AppUserModel>> GetById(string id) {
            try {
                var user = await userMgr.FindByIdAsync(id);
                if (user == null) {
                    return NotFound();
                }
                var model = Mapper.Map<AppUserModel>(user);
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
        public async Task<ActionResult<AppUserModel>> Update(
            [FromRoute]string id,
            [FromBody]AppUserModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
                if (user == null) {
                    return NotFound();
                }
                Mapper.Map(model, user);
                var result = await userMgr.UpdateAsync(user);
                if (result.Succeeded) {
                    Mapper.Map(user, model);
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
        public async Task<ActionResult> ResetPassword(
            [FromRoute]string id,
            [FromBody]ResetPasswordModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
        public async Task<ActionResult> LockUser(
            string id,
            DateTime lockEndTime
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
        public async Task<ActionResult> Unlock(string id) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
        public async Task<ActionResult<IList<AppRoleModel>>> GetUserRoles(
            string id
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
                if (user == null) {
                    return NotFound();
                }
                var roleNames = await userMgr.GetRolesAsync(user);
                var roles = await roleMgr.Roles
                        .Where(r => roleNames.Contains(r.Name))
                        .ToListAsync();
                var model = Mapper.Map<IList<AppRoleModel>>(roles);
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
        public async Task<ActionResult> AddUserToRole(
            [FromRoute]string id,
            [FromRoute]string roleName
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
        public async Task<ActionResult> RemoveUserFromRole(
            string id,
            string roleName
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id);
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
