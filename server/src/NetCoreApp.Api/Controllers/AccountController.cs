using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>账户 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private UserManager<AppUser> userMgr;
        private SignInManager<AppUser> signinMgr;

        public AccountController(
            UserManager<AppUser> userMgr,
            SignInManager<AppUser> signinMgr
        ) {
            this.userMgr = userMgr;
            this.signinMgr = signinMgr;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                userMgr = null;
                signinMgr = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>获取用户登录信息</summary>
        /// <response code="200">返回用户信息</response>
        /// <response code="403">用户未登录</response>
        /// <response code="404">用户不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("info")]
        public async Task<ActionResult<AccountInfoModel>> GetInfo() {
            try {
                if (!User.Identity.IsAuthenticated) {
                    return Forbid();
                }
                var user = await userMgr.FindByNameAsync(User.Identity.Name);
                if (user == null) {
                    return NotFound();
                }
                var model = new AccountInfoModel();
                model.Id = user.Id;
                model.UserName = user.UserName;
                model.Roles = await userMgr.GetRolesAsync(user);
                var claims = await userMgr.GetClaimsAsync(user);
                var surname = claims.FirstOrDefault(
                    c => c.Type == ClaimTypes.Surname
                );
                if (surname != null) {
                    model.Surname = surname.Value;
                }
                var givenName = claims.FirstOrDefault(
                    c => c.Type == ClaimTypes.GivenName
                );
                if (givenName != null) {
                    model.GivenName = givenName.Value;
                }
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get user account info.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>用户登录</summary>
        /// <response code="200">登录成功</response>
        /// <response code="400">登录失败，返回错误信息</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult> SignIn(
            [FromBody]AccountLoginModel model
        ) {
            try {
                var user = await userMgr.FindByNameAsync(model.UserName);
                if (user == null) {
                    return BadRequest($"用户 {model.UserName} 不存在!");
                }
                if (await userMgr.IsLockedOutAsync(user)) {
                    return BadRequest($"用户 {model.UserName} 已经被锁定!");
                }
                var hasPassword = await userMgr.HasPasswordAsync(user);
                if (!hasPassword) {
                    return BadRequest(
                        $"用户 {model.UserName} 没有设置密码， 无法使用密码登录!"
                    );
                }
                var isValid = await userMgr.CheckPasswordAsync(
                    user,
                    model.Password
                );
                if (!isValid) {
                    await userMgr.AccessFailedAsync(user);
                    return BadRequest(
                        $"输入的密码不正确， 请重试！"
                    );
                }
                return Ok();
            }
            catch (Exception ex) {
                logger.Error($"Can not signin user.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>注销</summary>
        /// <response code="204">注销成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> SignOut() {
            try {
                await signinMgr.SignOutAsync();
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error($"Can not sign out user.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

    }

}
