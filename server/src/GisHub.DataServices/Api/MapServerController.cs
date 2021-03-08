using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Esri;

namespace Beginor.GisHub.DataServices.Api {

    /// <summary>要素服务接口</summary>
    [ApiController]
    [Route("rest/services/features")]
    public class FeatureController : Controller {

        /// <summary>读取要素服务信息</summary>
        [HttpGet("{id:long}/MapServer/0")]
        public async Task<ActionResult> GetDescription(
            [FromRoute]long id,
            [FromQuery]AgsCommonParams param
        ) {
            throw new NotImplementedException();
        }

        [HttpGet("{id:long}/MapServer/0/query")]
        public async Task<ActionResult> QueryFeaturesByGetAsync(
            [FromRoute]long id,
            [FromQuery]AgsQueryParams query
        ) {
            throw new NotImplementedException();
        }

        [HttpPost("{id:long}/MapServer/0/query")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult> QueryFeaturesByPostAsync(
            [FromRoute]long id,
            [ModelBinder(BinderType = typeof(AgsQueryParamsBinder))]AgsQueryParams query
        ) {
            throw new NotImplementedException();
        }
    }

}
