using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.NetCoreApp.Common;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>验证码 API</summary>
[Route("api/captcha")]
[ApiController]
public class CaptchaController(
    ILogger<CaptchaController> logger,
    ICaptchaGenerator captcha
) : Controller {

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <response code="200">登录成功</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("")]
    public async Task<ActionResult> Captcha() {
        try {
            var result = await captcha.GenerateAsync();
            return File(result.Image, result.ContentType);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error while generating captcha!");
            return this.InternalServerError(ex);
        }
    }

}
