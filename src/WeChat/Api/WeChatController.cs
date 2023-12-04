using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NHibernate.Linq;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.WeChat;

/// <summary>微信授权相关API</summary>
[ApiController]
[Route("api/wechat")]
public class WeChatController : Controller {

    private readonly ILogger<WeChatController> logger;
    private readonly IAppLogRepository repository;
    private readonly WeChatOption option;
    private readonly ApiGateway apiGateway;
    private UserManager<AppUser> userMgr;
    private JwtOption jwt;
    private RoleManager<AppRole> roleMgr;
    private IDistributedCache cache;

    public WeChatController(
        ILogger<WeChatController> logger,
        IAppLogRepository repository,
        IOptions<WeChatOption> option,
        ApiGateway apiGateway,
        UserManager<AppUser> userMgr,
        IOptionsSnapshot<JwtOption> jwt,
        RoleManager<AppRole> roleMgr,
        IDistributedCache cache
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.option = option.Value ?? throw new ArgumentNullException(nameof(option));
        this.apiGateway = apiGateway ?? throw new ArgumentNullException(nameof(apiGateway));
        this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
        this.jwt = jwt.Value ?? throw new ArgumentNullException(nameof(jwt));
        this.roleMgr = roleMgr ?? throw new ArgumentNullException(nameof(roleMgr));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>小程序登录</summary>
    [HttpPost("auth-by-code")]
    public async Task<ActionResult<WeChatLoginResponseModel>> LoginByCode(
        [FromBody] WeChatLoginRequestModel model
    ) {
        var res = new WeChatLoginResponseModel {
            ErrCode = 0,
            ErrMsg = "ok",
        };
        if (string.IsNullOrWhiteSpace(model.Code)) {
            res.ErrCode = 40001;
            res.ErrMsg = $"参数 code 不能为空！";
            return BadRequest(res);
        }
        try {
            logger.LogInformation($"code is {model.Code}, ready to GetJsCode2SessionAsync.");
            var sessionRes = await apiGateway.GetJsCode2SessionAsync(model.Code);
            logger.LogInformation($"sessionRes is {sessionRes.ToJson()}.");
            var sessionId = $"wechat_session_{Guid.NewGuid():N}";
            res.SessionId = sessionId;
            await cache.SetStringAsync(sessionId, sessionRes.ToJson(), new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
            });
            var user = await userMgr.FindByLoginAsync(Consts.WeChatAuth, sessionRes.OpenId);
            return await JwtResultAsync(res, user);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not login by code : {model.ToJson()} from wechat.");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>小程序手机号登录</summary>
    [HttpPost("auth-by-phone")]
    public async Task<ActionResult<WeChatLoginResponseModel>> LoginByPhone(
        [FromBody] WeChatLoginByPhoneRequestModel model
    ) {
        var res = new WeChatLoginResponseModel {
            ErrCode = 0,
            ErrMsg = "ok",
        };
        if (string.IsNullOrWhiteSpace(model.SessionId) || string.IsNullOrWhiteSpace(model.EncryptedData) || string.IsNullOrWhiteSpace(model.IV)) {
            res.ErrCode = 40001;
            res.ErrMsg = $"参数有误！";
            return BadRequest(res);
        }
        try {
            logger.LogInformation($"SessionId is {model.SessionId}, ready to GetSession.");
            var sessionRes = await cache.GetObjectAsync<WeChatJsCode2SessionResponse>(model.SessionId);
            logger.LogInformation($"sessionRes is {sessionRes?.ToJson()}.");
            if (sessionRes == null) {
                res.ErrCode = 40002;
                res.ErrMsg = $"登录失败， 请重试!";
                return BadRequest(res);
            }
            res.SessionId = sessionRes.SessionKey;
            // 存在则直接登录
            var wxUser = await userMgr.FindByLoginAsync(Consts.WeChatAuth, sessionRes.OpenId);
            if (wxUser != null) {
                return await JwtResultAsync(res, wxUser);
            }
            // 解密手机号
            var phoneInfo = ProxyUtil.AESDecrypt<WeChatPhoneInfo>(model.EncryptedData, sessionRes.SessionKey, model.IV);
            logger.LogInformation($"phoneInfo is {phoneInfo?.ToJson()}.");
            if (phoneInfo == null || string.IsNullOrWhiteSpace(phoneInfo.PhoneNumber)) {
                res.ErrCode = 40002;
                res.ErrMsg = $"登录失败， 请重试!";
                return BadRequest(res);
            }
            var user = await userMgr.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneInfo.PhoneNumber);
            if (user != null) {
                // 绑定微信,方便后续登录,不再授权手机号
                await userMgr.AddLoginAsync(user, new UserLoginInfo(Consts.WeChatAuth, sessionRes.OpenId, user.UserName));
            }
            return await JwtResultAsync(res, user);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not login by phone : {model.ToJson()} from wechat.");
            return this.InternalServerError(ex);
        }
    }

    private async Task<ActionResult> JwtResultAsync(WeChatLoginResponseModel res, AppUser? user) {
        if (user == null) {
            res.ErrCode = 40002;
            res.ErrMsg = $"登录失败， 请重试!";
            return BadRequest(res);
        }
        if (await userMgr.IsLockedOutAsync(user)) {
            res.ErrCode = 40003;
            res.ErrMsg = $"用户 {user.UserName} 已经被锁定!";
            return BadRequest(res);
        }
        // update user last login and login count;
        user.LastLogin = DateTime.Now;
        user.LoginCount += 1;
        // user.AccessFailedCount = 0;
        await userMgr.UpdateAsync(user);

        var tmpToken = Guid.NewGuid().ToString("N");
        // 先暂存 token
        await cache.SetStringAsync(tmpToken, user.Id);
        res.TmpTokenKey = Beginor.NetCoreApp.Common.Consts.TmpToken;
        res.TmpTokenValue = tmpToken;
        return Ok(res);
    }
}
