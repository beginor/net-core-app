using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Beginor.GisHub.Common;

public class RolesFilterAttribute : ActionFilterAttribute {

    public string IdParameterName { get; set; }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        if (!context.ActionArguments.ContainsKey(IdParameterName)) {
            context.Result = new BadRequestObjectResult($"Parameter {IdParameterName} is not in action arguments.");
            return;
        }
        var provider = context.HttpContext.RequestServices.GetService<IRolesFilterProvider>();
        if (provider == null) {
            context.Result = new ObjectResult("required service IRolesFilterProvider is not registered!") {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }
        var id = context.ActionArguments[IdParameterName];
        var requiredRoles = await provider.GetRolesAsync(id);
        var userRoles = context.HttpContext.User.Claims.Where(
            c => c.Type == ClaimTypes.Role
        ).Select(c => c.Value).ToArray();
        if (!userRoles.Any(role => requiredRoles.Any(r => r == role))) {
            context.Result = new ForbidResult();
        }
        await base.OnActionExecutionAsync(context, next);
    }

}

public interface IRolesFilterProvider {

    Task<string[]> GetRolesAsync(object id);
    
    Task ResetRolesAsync(object id);

}
