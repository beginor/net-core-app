using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DynamicSql;

namespace Beginor.GisHub.DataServices.Api {

    partial class DataApiController {

        /// <summary>调用指定的数据API</summary>
        [Route("{id:long}/invoke")]
        [Authorize("data_apis.invoke")]
        public async Task<ActionResult> Invoke(long id) {
            try {
                var model = await repository.GetByIdAsync(id);
                if (model == null) {
                    return NotFound($"DataApi {id} does not exists.");
                }
                return Json(model);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not invoke api {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
