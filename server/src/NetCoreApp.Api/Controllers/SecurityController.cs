using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>安全相关的 API</summary>
    [Route("api/[controller]")]
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
        [HttpGet("xsrf-token")]
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
                    SameSite = SameSiteMode.None
                }
            );
            return Ok();
        }

    }

}
