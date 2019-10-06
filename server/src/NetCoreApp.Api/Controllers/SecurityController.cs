using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>安全相关的 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : Controller {

        private IAntiforgery antiforgery;

        public SecurityController(
            IAntiforgery antiforgery
        ) {
            this.antiforgery = antiforgery;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                antiforgery = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>获取跨站请求令牌</summary>
        /// <response code="200">获取成功，返回跨站请求令牌。</response>
        [HttpGet("antiforgery")]
        public ActionResult GetXsrfToken() {
            // todo: 根据信任主机来进行验证是否颁发 XSRF 令牌
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append(
                "XSRF-TOKEN",
                tokens.RequestToken,
                new CookieOptions {
                    HttpOnly = false,
                    Path = "/",
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                }
            );
            return Ok();
        }

    }

}
