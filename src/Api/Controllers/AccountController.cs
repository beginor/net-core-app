using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NHibernate.Linq;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Api.Authorization;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>账户 API</summary>
[Route("api/account")]
[ApiController]
public partial class AccountController(
    ILogger<AccountController> logger,
    UserManager<AppUser> userMgr,
    RoleManager<AppRole> roleMgr,
    IOptionsSnapshot<JwtOption> jwt,
    IAppNavItemRepository navRepo,
    IDistributedCache cache,
    IAppUserTokenRepository userTokenRepo,
    UsersController usersCtrl,
    IAppPrivilegeRepository privilegeRepo,
    ICaptchaGenerator captcha
) : Controller {

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // dispose managed resource here
        }
        base.Dispose(disposing);
    }

    /// <summary>获取用户登录信息</summary>
    /// <response code="200">返回用户信息</response>
    /// <response code="403">用户未登录</response>
    /// <response code="404">用户不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    [ResponseCache(NoStore = true, Duration = 0)]
    public async Task<ActionResult<AccountInfoModel>> GetInfo() {
        try {
            var userId = string.Empty;
            if (Request.Query.TryGetValue(Consts.TmpToken, out var value)) {
                var tmpToken = value.ToString();
                if (tmpToken.IsNotNullOrEmpty()) {
                    userId = await cache.GetStringAsync(tmpToken);
                    await cache.RemoveAsync(tmpToken);
                }
            }
            if (userId.IsNullOrEmpty()) {
                userId = this.GetUserId();
            }
            ClaimsIdentity? identity = null;
            if (userId.IsNotNullOrEmpty()) {
                var user = await userMgr.FindByIdAsync(userId!);
                if (user != null) {
                    identity = await CreateIdentityAsync(user);
                }
            }
            identity ??= await CreateAnonymousIdentity();
            var info = await CreateAccountInfoModel(identity);
            return info;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get user account info.");
            return this.InternalServerError(ex);
        }
    }

    [HttpGet("token")]
    [Authorize("account.get_info_by_token")]
    [ResponseCache(NoStore = true, Duration = 0)]
    public async Task<ActionResult<AccountInfoModel>> GetInfoByToken() {
        var identity = User.Identity as ClaimsIdentity;
        if (identity == null) {
            var anonymousIdentity = await CreateAnonymousIdentity();
            var anonymousInfo = await CreateAccountInfoModel(anonymousIdentity);
            anonymousInfo.Token = CreateJwtToken(anonymousIdentity);
            return anonymousInfo;
        }
        var userName = identity.Name;
        var idx = userName!.IndexOf(':');
        if (idx > -1) {
            userName = userName.Substring(0, idx);
        }
        var appUser = await userMgr.FindByNameAsync(userName);
        if (appUser == null) {
            return NotFound();
        }
        var userIdentity = await CreateIdentityAsync(appUser);
        var info = await CreateAccountInfoModel(userIdentity);
        info.Token = CreateJwtToken(userIdentity);
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
            model.UserName = SafeUrlEncoder.Decode(model.UserName);
            model.Password = SafeUrlEncoder.Decode(model.Password);
            var validCaptcha = await captcha.ValidateCodeAsync(model.Captcha);
            if (!validCaptcha) {
                return BadRequest("验证码错误，请重试！");
            }
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
            return this.InternalServerError(ex);
        }
    }

    private async Task<AccountInfoModel> CreateAccountInfoModel(ClaimsIdentity user) {
        var info = new AccountInfoModel {
            Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
            UserName = user.Claims.First(c => c.Type == ClaimTypes.Name).Value,
            Surname = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            GivenName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
        };
        var cachedClaims = await cache.GetUserClaimsAsync(info.Id);
        info.Roles = cachedClaims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct()
            .ToDictionary(r => r, _ => true);
        info.Privileges = cachedClaims
            .Where(c => c.Type == AppClaimTypes.Privilege)
            .Select(c => c.Value)
            .Distinct()
            .ToDictionary(p => p, _ => true);
        info.Token = CreateJwtToken(user);
        return info;
    }

    private string CreateJwtToken(ClaimsIdentity identity) {
        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor {
            Subject = identity,
            Expires = DateTime.UtcNow.Add(jwt.Value.ExpireTimeSpan),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwt.Value.SecretKey),
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
            new Claim(ClaimTypes.Name, user.UserName!)
        );
        // user claims;
        var userClaims = await userMgr.GetClaimsAsync(user);
        identity.AddClaims(userClaims);
        // save role and role privileges to cache;
        var claimsToCache = new List<Claim> {
            new (AppClaimTypes.OrganizeUnitId, user.OrganizeUnit.Id.ToString()),
            new (AppClaimTypes.OrganizeUnitCode, user.OrganizeUnit.Code),
        };
        // role as claim;
        var roles = await userMgr.GetRolesAsync(user);
        // add role and role claims;
        foreach (var roleName in roles) {
            claimsToCache.Add(new Claim(ClaimTypes.Role, roleName));
            var role = await roleMgr.FindByNameAsync(roleName);
            if (role == null) {
                continue;
            }
            var roleClaims = await roleMgr.GetClaimsAsync(role);
            foreach (var roleClaim in roleClaims) {
                if (!identity.Claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value)) {
                    claimsToCache.Add(roleClaim);
                }
            }
            await cache.SetUserClaimsAsync(user.Id, claimsToCache.ToArray(), jwt.Value.ExpireTimeSpan);
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
        if (roles != null) {
            foreach (var role in roles) {
                claimsToCache.Add(new Claim(ClaimTypes.Role, role.Name!));
                // var role = await roleMgr.FindByNameAsync(roleName);
                var roleClaims = await roleMgr.GetClaimsAsync(role);
                foreach (var roleClaim in roleClaims) {
                    if (!identity.Claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value)) {
                        claimsToCache.Add(roleClaim);
                    }
                }
            }
        }
        await cache.SetUserClaimsAsync("anonymous", claimsToCache.ToArray(), jwt.Value.ExpireTimeSpan);
        return identity;
    }
}
