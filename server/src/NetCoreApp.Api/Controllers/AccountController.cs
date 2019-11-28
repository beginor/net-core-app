using System;
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
                // var identity = User.Identity as ClaimsIdentity;
                var identity = await CreateIdentityAsync(appUser);
                var info = CreateAccountInfoModel(identity);
                info.Token = CreateJwtToken(identity);
                return info;
            }
            catch (Exception ex) {
                logger.Error($"Can not get user account info.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        private AccountInfoModel CreateAccountInfoModel(ClaimsIdentity user) {
            var info = new AccountInfoModel {
                Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                UserName = user.Claims.First(c => c.Type == ClaimTypes.Name).Value,
                Surname = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                GivenName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                Roles = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value?.Split(",")?.ToDictionary(r => r, r => true),
                Privileges = user.Claims
                    .Where(c => c.Type == Consts.PrivilegeClaimType)
                    .Select(c => c.Value)
                    .Distinct()
                    .ToDictionary(p => p, p => true)
            };
            return info;
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
                user.AccessFailedCount = 0;
                await userMgr.UpdateAsync(user);
                var identity = await CreateIdentityAsync(user);
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
            var descriptor = new SecurityTokenDescriptor {
                Subject = identity,
                Expires = DateTime.UtcNow.Add(jwt.ExpireTimeSpan),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwt.SecretKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var securityToken = handler.CreateToken(descriptor);
            var jwtToken = handler.WriteToken(securityToken);
            return jwtToken;
        }

        private async Task<ClaimsIdentity> CreateIdentityAsync(AppUser user) {
            // create a identity;
            var identity = new ClaimsIdentity();
            identity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            );
            identity.AddClaim(
                new Claim(ClaimTypes.Name, user.UserName)
            );
            // user claims;
            var userClaims = await userMgr.GetClaimsAsync(user);
            identity.AddClaims(userClaims);
            // role as claim;
            var roles = await userMgr.GetRolesAsync(user);
            identity.AddClaim(
                new Claim(ClaimTypes.Role, string.Join(',', roles))
            );
            // role claims;
            foreach (var roleName in roles) {
                var role = await roleMgr.FindByNameAsync(roleName);
                var roleClaims = await roleMgr.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims) {
                    if (!identity.Claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value)) {
                        identity.AddClaim(roleClaim);
                    }
                }
            }
            return identity;
        }

    }

}
