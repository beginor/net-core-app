using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gmap {

    public class EnableRequestBufferingAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(ActionExecutingContext context) {
            context.HttpContext.Request.EnableBuffering();
        }

    }

}
