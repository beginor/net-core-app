using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;
using Beginor.GisHub.Data;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.Models;

namespace Beginor.GisHub.Api.Controllers {

    /// <summary>用户 API</summary>
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller {

        private ILogger<UsersController> logger;
        private UserManager<AppUser> userMgr;
        private RoleManager<AppRole> roleMgr;
        private IMapper mapper;

        public UsersController(
            ILogger<UsersController> logger,
            UserManager<AppUser> userMgr,
            RoleManager<AppRole> roleMgr,
            IMapper mapper
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            this.roleMgr = roleMgr ?? throw new ArgumentNullException(nameof(roleMgr));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
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
        [Authorize("app_users.create")]
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
                user = new AppUser();
                var claims = user.UpdateFromUserModel(mapper, model);
                var result = await userMgr.CreateAsync(user);
                // add default roles to new created user;
                var defaultRoles = await roleMgr.Roles
                    .Where(r => r.IsDefault == true)
                    .Select(r => r.Name)
                    .ToListAsync();
                await userMgr.AddToRolesAsync(user, defaultRoles);
                await AddOrReplaceClaims(user, claims);
                if (result.Succeeded) {
                    mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not create user with {model.ToJson()}");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>删除指定的用户</summary>
        /// <response code="204">删除用户成功</response>
        /// <response code="400">删除用户出错</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_users.delete")]
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
                logger.LogError(ex, $"Can not delete user by id {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>获取用户列表</summary>
        /// <response code="200">获取成功并返回用户列表。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpGet("")]
        [Authorize("app_users.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppUserModel>>> GetAll(
            [FromQuery]UserSearchRequestModel model
        ) {
            try {
                IQueryable<AppUser> query;
                if (string.IsNullOrEmpty(model.RoleName)) {
                    query = userMgr.Users;
                }
                else {
                    var usersInRole = await userMgr.GetUsersInRoleAsync(model.RoleName);
                    query = usersInRole.AsQueryable();
                }
                if (model.UserName.IsNotNullOrEmpty()) {
                    query = query.Where(u => u.UserName.Contains(model.UserName));
                }
                var total = query.LongCount();
                var sortInfo = model.SortBy.Split(
                    ':',
                    StringSplitOptions.RemoveEmptyEntries
                );
                var propertyName = sortInfo[0];
                var isAsc = sortInfo[1].Equals(
                    "ASC",
                    StringComparison.OrdinalIgnoreCase
                );
                var data = query
                    .AddOrderBy(sortInfo[0], isAsc)
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .ToList();
                var models = new List<AppUserModel>();
                foreach (var user in data) {
                    var claims = await userMgr.GetClaimsAsync(user);
                    var userModel = user.ToUserModel(mapper, claims);
                    models.Add(userModel);
                }
                var result = new PaginatedResponseModel<AppUserModel> {
                    Skip = model.Skip,
                    Take = model.Take,
                    Total = total,
                    Data = models
                };
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search user with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>获取指定用户</summary>
        /// <response code="404">找不到指定的用户。</response>
        /// <response code="200">获取用户成功，返回用户信息。</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("app_users.read_by_id")]
        public async Task<ActionResult<AppUserModel>> GetById(long id) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var claims = await userMgr.GetClaimsAsync(user);
                var model = user.ToUserModel(mapper, claims);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get user with id {id}");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>更新指定的用户</summary>
        /// <response code="200">更新成功并返回用户信息</response>
        /// <response code="400">更新用户出错并返回用户信息</response>
        /// <response code="404">指定的用户不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("app_users.update")]
        public async Task<ActionResult<AppUserModel>> Update(
            [FromRoute]long id,
            [FromBody]AppUserModel model
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var claims = user.UpdateFromUserModel(mapper, model);
                await AddOrReplaceClaims(user, claims);
                var result = await userMgr.UpdateAsync(user);
                if (result.Succeeded) {
                    mapper.Map(user, model);
                    return model;
                }
                return BadRequest(result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update user by id {id} with {model.ToJson()}");
                return this.InternalServerError(ex);
            }
        }

        private async Task AddOrReplaceClaims(AppUser user, IList<Claim> claims) {
            var userClaims = await userMgr.GetClaimsAsync(user);
            foreach (var claim in claims) {
                var userClaim = userClaims.FirstOrDefault(c => c.Type == claim.Type);
                if (userClaim == null) {
                    await userMgr.AddClaimAsync(user, claim);
                }
                else {
                    await userMgr.ReplaceClaimAsync(user, userClaim, claim);
                }
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
        [Authorize("app_users.reset_pass")]
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
                logger.LogError(ex, $"Can not reset password for user {id} with {model.ToJson()}");
                return this.InternalServerError(ex);
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
        [Authorize("app_users.lock")]
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
                var offset = new DateTimeOffset(
                    lockEndTime,
                    TimeZoneInfo.Local.GetUtcOffset(lockEndTime)
                );
                await userMgr.SetLockoutEndDateAsync(user, offset);
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not Lock user by id {id} to {lockEndTime}");
                return this.InternalServerError(ex);
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
        [Authorize("app_users.unlock")]
        public async Task<ActionResult> Unlock(long id) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                // await manager.SetLockoutEnabledAsync(user, false);
                await userMgr.SetLockoutEndDateAsync(
                    user,
                    null
                );
                await userMgr.ResetAccessFailedCountAsync(user);
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not unlock user by id {id}");
                return this.InternalServerError(ex);
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
        [Authorize("app_users.read_user_roles")]
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
                logger.LogError(ex, $"Can not get roles for user {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 添加用户角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="roleNames">角色名称列表，多个之间用`,`分割</param>
        /// <response code="200">添加用户角色用户成功。</response>
        /// <response code="404">用户或角色不存在。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpPut("{id:long}/roles/{roleNames}")]
        [Authorize("app_users.add_role_to_user")]
        public async Task<ActionResult> AddUserToRole(
            [FromRoute]long id,
            [FromRoute]string roleNames
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var arr = roleNames.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in arr) {
                    var exists = await roleMgr.RoleExistsAsync(role);
                    if (exists) {
                        var isInRole = await userMgr.IsInRoleAsync(user, role);
                        if (!isInRole) {
                            await userMgr.AddToRoleAsync(user, role);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not add user {id} to role {roleNames}", ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="roleNames">角色名称，多个之间用`,`分割</param>
        /// <response code="204">删除用户角色成功。</response>
        /// <response code="404">用户或角色不存在。</response>
        /// <response code="400">删除用户角色出错。</response>
        /// <response code="500">服务器内部错误。</response>
        [HttpDelete("{id:long}/roles/{roleNames}")]
        [ProducesResponseType(204)]
        [Authorize("app_users.remove_role_from_user")]
        public async Task<ActionResult> RemoveUserFromRole(
            long id,
            string roleNames
        ) {
            try {
                var user = await userMgr.FindByIdAsync(id.ToString());
                if (user == null) {
                    return NotFound();
                }
                var arr = roleNames.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in arr) {
                    var isInRole = await userMgr.IsInRoleAsync(user, role);
                    if (isInRole) {
                        await userMgr.RemoveFromRoleAsync(user, role);
                    }
                }
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not remove user {id} from role {roleNames} .", ex);
                return this.InternalServerError(ex);
            }
        }

    }

}
