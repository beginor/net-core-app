using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NHibernate.Linq;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;

namespace Beginor.NetCoreApp.WeChat.Api;

/// <summary>微信授权相关API</summary>
[ApiController]
[Route("api/wechat")]
public class WeChatController : Controller {

    private readonly ILogger<WeChatController> logger;
    private readonly ApiGateway apiGateway;
    private UserManager<AppUser> userMgr;
    private IDistributedCache cache;

    public WeChatController(
        ILogger<WeChatController> logger,
        ApiGateway apiGateway,
        UserManager<AppUser> userMgr,
        IDistributedCache cache
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.apiGateway = apiGateway ?? throw new ArgumentNullException(nameof(apiGateway));
        this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>小程序登录</summary>
    [HttpPost("auth")]
    public async Task<ActionResult<WeChatLoginResponseModel>> LoginByPhone(
        [FromBody] WeChatLoginByPhoneRequestModel model
    ) {
        var res = new WeChatLoginResponseModel {
            ErrCode = 0,
            ErrMsg = "ok",
        };
        try {
            logger.LogInformation($"code is {model.Code}, ready to GetJsCode2SessionAsync.");

            var (session, sessionId) = await GetWeChatSessionAsync(model.Code, model.SessionId);
            res.SessionId = sessionId;
            logger.LogInformation($"sessionRes is {session.ToJson()}.");

            var user = await userMgr.FindByLoginAsync(Consts.WeChatAuth, session.OpenId);
            if (user != null) {
                return await JwtResultAsync(res, user);
            }
            // 需要微信用户的手机号， 在客户端显式调用用手机号登录的对话框；
            if (model.EncryptedData.IsNullOrEmpty() || model.IV.IsNullOrEmpty()) {
                res.ErrCode = 40002;
                res.ErrMsg = "登录失败， 请重试!";
                return BadRequest(res);
            }
            var phoneInfo = ProxyUtil.AESDecrypt<WeChatPhoneInfo>(
                model.EncryptedData, session.SessionKey, model.IV
            );
            logger.LogInformation($"phoneInfo is {phoneInfo?.ToJson()}.");
            if (phoneInfo == null || string.IsNullOrWhiteSpace(phoneInfo.PhoneNumber)) {
                res.ErrCode = 40002;
                res.ErrMsg = "登录失败， 请重试!";
                return BadRequest(res);
            }
            user = await userMgr.Users.FirstOrDefaultAsync(
                u => u.PhoneNumber == phoneInfo.PhoneNumber
            );
            if (user != null) {
                // 绑定微信,方便后续登录,不再授权手机号
                await userMgr.AddLoginAsync(
                    user,
                    new UserLoginInfo(
                        Consts.WeChatAuth, session.OpenId, user.UserName
                    )
                );
            }
            return await JwtResultAsync(res, user);
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not login by phone : {model.ToJson()} from wechat.");
            return this.InternalServerError(ex);
        }
    }

    private async Task<(WeChatJsCode2SessionResponse, string)> GetWeChatSessionAsync(string code, string sessionId) {
        if (sessionId.IsNotNullOrEmpty()) {
            var cachedSession = await cache.GetAsync<WeChatJsCode2SessionResponse>(sessionId);
            if (cachedSession != null) {
                return (cachedSession, sessionId);
            }
        }
        var session = await apiGateway.GetJsCode2SessionAsync(code);
        var newSessionId = $"wechat_session_{Guid.NewGuid():N}";
        await cache.SetAsync(newSessionId, session, TimeSpan.FromMinutes(60));
        return (session, newSessionId);
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
        res.TmpTokenKey = Consts.TmpToken;
        res.TmpTokenValue = tmpToken;
        return Ok(res);
    }
}
