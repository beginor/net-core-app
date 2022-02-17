using System;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.Common;
using Beginor.GisHub.Data.Entities;
using Beginor.GisHub.DataServices;
using Beginor.GisHub.DynamicSql.Data;
using Beginor.GisHub.DynamicSql.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.DynamicSql.Api {

    /// <summary>数据API 服务接口</summary>
    [ApiController]
    [Route("api/dataapis")]
    public partial class DataApiController : Controller {

        private ILogger<DataApiController> logger;
        private IDataApiRepository repository;
        private UserManager<AppUser> userMgr;
        private JsonSerializerOptionsFactory serializerOptionsFactory;
        private ParameterConverterFactory parameterConverterFactory;

        public DataApiController(
            ILogger<DataApiController> logger,
            IDataApiRepository repository,
            UserManager<AppUser> userMgr,
            JsonSerializerOptionsFactory serializerOptionsFactory,
            ParameterConverterFactory parameterConverterFactory
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
            this.serializerOptionsFactory = serializerOptionsFactory ?? throw new ArgumentNullException(nameof(serializerOptionsFactory));
            this.parameterConverterFactory = parameterConverterFactory ?? throw new ArgumentNullException(nameof(parameterConverterFactory));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                userMgr = null;
                serializerOptionsFactory = null;
                parameterConverterFactory = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>搜索 数据API ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("data_apis.read")]
        public async Task<ActionResult<PaginatedResponseModel<DataApiModel>>> Search(
            [FromQuery]DataApiSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search data_apis with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary> 创建 数据API </summary>
        /// <response code="200">创建 数据API 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("data_apis.create")]
        public async Task<ActionResult<DataApiModel>> Create(
            [FromBody]DataApiModel model
        ) {
            try {
                var userId = this.GetUserId();
                var user = await userMgr.FindByIdAsync(userId);
                if (user == null) {
                    return BadRequest("User is null!");
                }
                await repository.SaveAsync(model, user);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to data_apis.");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>删除 数据API </summary>
        /// <response code="204">删除 数据API 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("data_apis.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                var userId = this.GetUserId();
                var user = await userMgr.FindByIdAsync(userId);
                if (user == null) {
                    return BadRequest("User is null!");
                }
                await repository.DeleteAsync(id, user);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete data_apis by id {id} .");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 获取指定的 数据API
        /// </summary>
        /// <response code="200">返回 数据API 信息</response>
        /// <response code="404"> 数据API 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("data_apis.read_by_id")]
        public async Task<ActionResult<DataApiModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get data_apis by id {id}.");
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        /// 更新 数据API
        /// </summary>
        /// <response code="200">更新成功，返回 数据API 信息</response>
        /// <response code="404"> 数据API 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("data_apis.update")]
        public async Task<ActionResult<DataApiModel>> Update(
            [FromRoute]long id,
            [FromBody]DataApiModel model
        ) {
            try {
                var exists = await repository.ExistAsync(id);
                if (!exists) {
                    return NotFound();
                }
                var userId = this.GetUserId();
                var user = await userMgr.FindByIdAsync(userId);
                if (user == null) {
                    return BadRequest("User is null!");
                }
                await repository.UpdateAsync(id, model, user);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update data_apis by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex);
            }
        }

    }

}
