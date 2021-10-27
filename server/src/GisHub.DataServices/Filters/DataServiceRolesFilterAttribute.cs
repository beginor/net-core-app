using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Filters {

    public class DataServiceRolesFilterAttribute : ActionFilterAttribute {

        public string IdParameterName { get; set; }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        ) {
            if (!context.ActionArguments.ContainsKey(IdParameterName)) {
                context.Result = new BadRequestObjectResult($"Parameter {IdParameterName} is not in action arguments.");
            }
            else {
                var id = (long) context.ActionArguments[IdParameterName];
                var repo = context.HttpContext.RequestServices.GetService<IDataServiceRepository>();
                if (repo != null) {
                    var cachedItem = await repo.GetCacheItemByIdAsync(id);
                    var userRoles = context.HttpContext.User.Claims.Where(
                        c => c.Type == ClaimTypes.Role
                    ).Select(c => c.Value).ToArray();
                    if (!userRoles.Any(role => cachedItem.Roles.Any(r => r == role))) {
                        context.Result = new ForbidResult();
                    }
                }
            }
            await base.OnActionExecutionAsync(context, next);
        }

    }

}
