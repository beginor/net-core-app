using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Beginor.GisHub.Common;

namespace Beginor.GisHub.DataServices.Filters {

    public class CompressResultAttribute : ActionFilterAttribute {

        public override void OnResultExecuting(ResultExecutingContext context) {
            var result = context.Result;
            var commonOption = context.HttpContext.RequestServices.GetService<CommonOption>();
            if (!commonOption.Output.Compress) {

            }
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context) {
            base.OnResultExecuted(context);
        }

    }

}
