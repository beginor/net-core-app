using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    partial class AccountController {

        /// <summary>获取当前用户信息</summary>
        [HttpGet("user")] // GET /api/account/user
        [Authorize]
        public async Task<ActionResult<AppUserModel>> GetUser() {
            var userIdStr = User.GetUserId();
            if (userIdStr.IsNullOrEmpty()) {
                return Forbid();
            }
            if (!long.TryParse(userIdStr, out var userId)) {
                return Forbid();
            }
            return await usersCtrl.GetById(userId);
        }

        /// <summary>更新当前用户信息</summary>
        [HttpPut("user")] // PUT /api/account/user;
        [Authorize]
        public async Task<ActionResult<AppUserModel>> UpdateUser([FromBody]AppUserModel model) {
            var userIdStr = User.GetUserId();
            if (userIdStr.IsNullOrEmpty()) {
                return Forbid();
            }
            if (!long.TryParse(userIdStr, out var userId)) {
                return Forbid();
            }
            return await usersCtrl.Update(userId, model);
        }

        /// <summary>修改当前账户密码</summary>
        [HttpPut("password")] // PUT /api/account/password
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordModel model) {
            var userId = User.GetUserId();
            var user = await userMgr.FindByIdAsync(userId);
            if (user == null) {
                return Forbid();
            }
            try {
                model.CurrentPassword = Encoding.UTF8.GetString(Convert.FromBase64String(model.CurrentPassword));
                model.NewPassword = Encoding.UTF8.GetString(Convert.FromBase64String(model.NewPassword));
                model.ConfirmPassword = Encoding.UTF8.GetString(Convert.FromBase64String(model.ConfirmPassword));
                var isValid = await userMgr.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isValid) {
                    return BadRequest("Invalid current password!");
                }
                var result = await userMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded) {
                    return Ok();
                }
                else {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not change password for user {user.UserName} with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }
    }

}
