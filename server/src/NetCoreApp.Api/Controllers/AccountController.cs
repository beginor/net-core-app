using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>账户 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller {

        private ILogger<AccountController> logger;
        private UserManager<AppUser> userMgr;
        private RoleManager<AppRole> roleMgr;
        private JwtOption jwt;
        private IAppNavItemRepository navRepo;
        private ICache cache;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<AppUser> userMgr,
            RoleManager<AppRole> roleMgr,
            IOptionsSnapshot<JwtOption> jwt,
            IAppNavItemRepository navRepo,
            ICache cache
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            this.roleMgr = roleMgr ?? throw new ArgumentNullException(nameof(roleMgr));
            this.jwt = jwt.Value ?? throw new ArgumentNullException(nameof(jwt));
            this.navRepo = navRepo ?? throw new ArgumentNullException(nameof(navRepo));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                userMgr = null;
                roleMgr = null;
                jwt = null;
                navRepo = null;
                cache = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>获取用户登录信息</summary>
        /// <response code="200">返回用户信息</response>
        /// <response code="403">用户未登录</response>
        /// <response code="404">用户不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("info")]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult<AccountInfoModel>> GetInfo() {
            try {
                if (!User.Identity.IsAuthenticated || User.HasClaim(ClaimTypes.NameIdentifier, string.Empty)) {
                    var anonymousIdentity = await CreateAnonymousIdentity();
                    var anonymousInfo = CreateAccountInfoModel(anonymousIdentity);
                    anonymousInfo.Token = CreateJwtToken(anonymousIdentity);
                    return anonymousInfo;
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
                logger.LogError(ex, $"Can not get user account info.");
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
                model.UserName = Encoding.UTF8.GetString(Convert.FromBase64String(model.UserName));
                model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
                var user = await userMgr.FindByNameAsync(model.UserName);
                if (user == null) {
                    return BadRequest($"登录失败， 请重试!");
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
                        $"登录失败， 请重试！"
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
                logger.LogError(ex, $"Can not signin user {model.ToJson()}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("menu")]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<MenuNodeModel> GetMenuAsync() {
            try {
                IList<string> roles;
                if (!User.Identity.IsAuthenticated || User.HasClaim(ClaimTypes.NameIdentifier, string.Empty)) {
                    roles = roleMgr.Roles
                        .Where(role => role.IsAnonymous == true)
                        .Select(role => role.Name)
                        .ToList();
                }
                else {
                    var user = await userMgr.FindByNameAsync(User.Identity.Name);
                    roles = await userMgr.GetRolesAsync(user);
                }
                var menuModel = await navRepo.GetMenuAsync(roles.ToArray());
                return menuModel;
            }
            catch (Exception ex) {
                logger.LogError(ex, "Can not get menu!");
                return new MenuNodeModel();
            }
        }

        private AccountInfoModel CreateAccountInfoModel(ClaimsIdentity user) {
            var info = new AccountInfoModel {
                Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                UserName = user.Claims.First(c => c.Type == ClaimTypes.Name).Value,
                Surname = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                GivenName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                Roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .Distinct()
                    .ToDictionary(r => r, r => true),
                Privileges = user.Claims
                    .Where(c => c.Type == Consts.PrivilegeClaimType)
                    .Select(c => c.Value)
                    .Distinct()
                    .ToDictionary(p => p, p => true)
            };
            return info;
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
            // save role and role privileges to cache;
            var claimsToCache = new List<Claim>();
            // role as claim;
            var roles = await userMgr.GetRolesAsync(user);
            // add role and role claims;
            foreach (var roleName in roles) {
                claimsToCache.Add(new Claim(ClaimTypes.Role, roleName));
                var role = await roleMgr.FindByNameAsync(roleName);
                var roleClaims = await roleMgr.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims) {
                    if (!identity.Claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value)) {
                        claimsToCache.Add(roleClaim);
                    }
                }
                await cache.SetItemAsync(user.Id, claimsToCache.ToArray());
            }
            return identity;
        }

        private async Task<ClaimsIdentity> CreateAnonymousIdentity() {
            var identity = new ClaimsIdentity();
            identity.AddClaim(
                new Claim(ClaimTypes.NameIdentifier, string.Empty)
            );
            identity.AddClaim(
                new Claim(ClaimTypes.Name, "anonymous")
            );
            identity.AddClaim(
                new Claim(ClaimTypes.Surname, "匿名")
            );
            identity.AddClaim(
                new Claim(ClaimTypes.GivenName, "用户")
            );
            // save role and role privileges to cache;
            var claimsToCache = new List<Claim>();
            // role as claim;
            var roles = await roleMgr.Roles
                .Where(r => r.IsAnonymous == true)
                .ToListAsync();
            // add role and role claims;
            foreach (var role in roles) {
                claimsToCache.Add(new Claim(ClaimTypes.Role, role.Name));
                // var role = await roleMgr.FindByNameAsync(roleName);
                var roleClaims = await roleMgr.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims) {
                    if (!identity.Claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value)) {
                        claimsToCache.Add(roleClaim);
                    }
                }
            }
            await cache.SetItemAsync("anonymous", claimsToCache.ToArray());
            return identity;
        }
    }

}
