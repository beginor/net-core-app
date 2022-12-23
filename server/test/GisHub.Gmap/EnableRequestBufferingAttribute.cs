using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Beginor.GisHub.Gmap;

public class EnableRequestBufferingAttribute : ActionFilterAttribute {

    public override void OnActionExecuting(ActionExecutingContext context) {
        context.HttpContext.Request.EnableBuffering();
    }

}
