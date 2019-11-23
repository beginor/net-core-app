using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>账户 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private UserManager<AppUser> userMgr;
        private RoleManager<AppRole> roleMgr;
        private Jwt jwt;

        public AccountController(
            UserManager<AppUser> userMgr,
            RoleManager<AppRole> roleMgr,
            Jwt jwt
        ) {
            this.userMgr = userMgr;
            this.roleMgr = roleMgr;
            this.jwt = jwt;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                userMgr = null;
                roleMgr = null;
                jwt = null;
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
                var appUser = await userMgr.FindByNameAsync(User.Identity.Name);
                if (appUser == null) {
                    return NotFound();
                }
                var model = new AccountInfoModel {
                    Id = appUser.Id,
                    UserName = appUser.UserName
                };
                var roles = await userMgr.GetRolesAsync(appUser);
                model.Roles = roles.ToDictionary(r => r, r => true);
                model.Privileges = new Dictionary<string, bool>();
                foreach (var roleName in roles) {
                    var role = await roleMgr.FindByNameAsync(roleName);
                    var roleClaims = await roleMgr.GetClaimsAsync(role);
                    var privileges = roleClaims.Where(
                        claim => claim.Type == Consts.PrivilegeClaimType
                    ).Select(claim => claim.Value);
                    foreach (var privilege in privileges) {
                        if (!model.Privileges.ContainsKey(privilege)) {
                            model.Privileges.Add(privilege, true);
                        }
                    }
                }
                var surname = User.Claims.FirstOrDefault(
                    c => c.Type == ClaimTypes.Surname
                );
                if (surname != null) {
                    model.Surname = surname.Value;
                }
                var givenName = User.Claims.FirstOrDefault(
                    c => c.Type == ClaimTypes.GivenName
                );
                if (givenName != null) {
                    model.GivenName = givenName.Value;
                }
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get user account info.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
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
                // update user last login and login count;
                user.LastLogin = DateTime.Now;
                user.LoginCount += 1;
                await userMgr.UpdateAsync(user);
                // create a identity;
                var identity = new ClaimsIdentity();
                identity.AddClaim(
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                );
                identity.AddClaim(
                    new Claim(ClaimTypes.Name, user.UserName)
                );
                // create a jwt token;
                var result = CreateJwtToken(identity);
                return Ok(result);
            }
            catch (Exception ex) {
                logger.Error($"Can not signin user.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        private string CreateJwtToken(ClaimsIdentity identity) {
            var handler = new JwtSecurityTokenHandler();
            var desc = new SecurityTokenDescriptor {
                Subject = identity,
                Expires = DateTime.UtcNow.Add(jwt.ExpireTimeSpan),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwt.SecretKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var securityToken = handler.CreateToken(desc);
            var jwtToken = handler.WriteToken(securityToken);
            return jwtToken;
        }

    }

}
