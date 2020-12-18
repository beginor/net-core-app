using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.DataServices.Models;
using Beginor.GisHub.DataServices.Data;

namespace Beginor.GisHub.DataServices.Api {

    /// <summary>数据库连接串 服务接口</summary>
    [ApiController]
    [Route("api/connections")]
    public class ConnectionController : Controller {

        private ILogger<ConnectionController> logger;
        private IConnectionRepository repository;

        public ConnectionController(
            ILogger<ConnectionController> logger,
            IConnectionRepository repository
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

        /// <summary>搜索 数据库连接串 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("connection_strings.read")]
        public async Task<ActionResult<PaginatedResponseModel<ConnectionModel>>> Search(
            [FromQuery]ConnectionStringSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search connection_strings with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        [HttpGet("~/api/connection-strings-list")]
        [Authorize("connection_strings.read")]
        /// <summary>获取全部的数据库连接列表</summary>
        public async Task<ActionResult<List<ConnectionModel>>> GetAllForDisplayAsync() {
            try {
                var result = await repository.GetAllForDisplayAsync();
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, "Can not get connection-strings-list .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }


        /// <summary> 创建 数据库连接串 </summary>
        /// <response code="200">创建 数据库连接串 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("connection_strings.create")]
        public async Task<ActionResult<ConnectionModel>> Create(
            [FromBody]ConnectionModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to connection_strings.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 数据库连接串 </summary>
        /// <response code="204">删除 数据库连接串 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("connection_strings.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete connection_strings by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 数据库连接串
        /// </summary>
        /// <response code="200">返回 数据库连接串 信息</response>
        /// <response code="404"> 数据库连接串 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("connection_strings.read_by_id")]
        public async Task<ActionResult<ConnectionModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get connection_strings by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 数据库连接串
        /// </summary>
        /// <response code="200">更新成功，返回 数据库连接串 信息</response>
        /// <response code="404"> 数据库连接串 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("connection_strings.update")]
        public async Task<ActionResult<ConnectionModel>> Update(
            [FromRoute]long id,
            [FromBody]ConnectionModel model
        ) {
            try {
                var exists = await repository.ExitsAsync(id);
                if (!exists) {
                    return NotFound();
                }
                await repository.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update connection_strings by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
