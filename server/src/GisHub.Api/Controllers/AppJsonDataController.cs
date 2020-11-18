using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Models;
using Beginor.GisHub.Data.Repositories;

namespace Beginor.GisHub.Api.Controllers {

    /// <summary>json 数据 服务接口</summary>
    [ApiController]
    [Route("api/json")]
    public class AppJsonDataController : Controller {

        private ILogger<AppJsonDataController> logger;
        private IAppJsonDataRepository repository;

        public AppJsonDataController(
            ILogger<AppJsonDataController> logger,
            IAppJsonDataRepository repository
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>删除 json 数据 </summary>
        /// <response code="204">删除 json 数据 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_json_data.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete app_json_data by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 json 数据
        /// </summary>
        /// <response code="200">返回 json 数据 信息</response>
        /// <response code="404"> json 数据 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("app_json_data.read")]
        public async Task<ActionResult> GetById(long id) {
            try {
                var value = await repository.GetValueByIdAsync(id);
                if (value.ValueKind == JsonValueKind.Undefined) {
                    return NotFound();
                }
                return Ok(value);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get app_json_data by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 json 数据
        /// </summary>
        /// <response code="200">更新成功，返回 json 数据 信息</response>
        /// <response code="404"> json 数据 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("app_json_data.update")]
        public async Task<ActionResult> Update(
            [FromRoute]long id,
            [FromBody]JsonElement model
        ) {
            try {
                var exists = await repository.ExitsAsync(id);
                if (!exists) {
                    return NotFound();
                }
                await repository.SaveValueAsync(id, model);
                return Ok();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update app_json_data by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
