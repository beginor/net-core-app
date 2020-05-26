using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Data.Repositories;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>程序客户端错误记录 服务接口</summary>
    [ApiController]
    [Route("api/client-errors")]
    public class AppClientErrorController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppClientErrorRepository repo;

        public AppClientErrorController(IAppClientErrorRepository repo) {
            this.repo = repo;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repo = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 程序客户端错误记录 </summary>
        /// <response code="200">创建 程序客户端错误记录 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("app_client_errors.create")]
        public async Task<ActionResult<AppClientErrorModel>> Create(
            [FromBody]AppClientErrorModel model
        ) {
            try {
                await repo.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create client_errors.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 程序客户端错误记录 </summary>
        /// <response code="204">删除 程序客户端错误记录 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_client_errors.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error("Can not delete client_errors.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 程序客户端错误记录 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("client_errors.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppClientErrorModel>>> GetAll(
            [FromQuery]AppClientErrorSearchModel model
        ) {
            try {
                var result = await repo.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all client_errors .", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 程序客户端错误记录
        /// </summary>
        /// <response code="200">返回 程序客户端错误记录 信息</response>
        /// <response code="404"> 程序客户端错误记录 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("client_errors.read")]
        public async Task<ActionResult<AppClientErrorModel>> GetById(long id) {
            try {
                var result = await repo.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get client_errors with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 程序客户端错误记录
        /// </summary>
        /// <response code="200">更新成功，返回 程序客户端错误记录 信息</response>
        /// <response code="404"> 程序客户端错误记录 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("client_errors.update")]
        public async Task<ActionResult<AppClientErrorModel>> Update(
            [FromRoute]long id,
            [FromBody]AppClientErrorModel model
        ) {
            try {
                var modelInDb = await repo.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await repo.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update client_errors with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
