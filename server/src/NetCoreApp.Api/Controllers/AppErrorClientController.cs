using System;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>客户端错误服务接口</summary>
    [Route("api/client-errors")]
    [ApiController]
    public class AppClientErrorController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppClientErrorRepository repository;

        public AppClientErrorController(IAppClientErrorRepository repository) {
            this.repository = repository;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 客户端错误记录 </summary>
        /// <response code="200">创建 客户端错误记录 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        // [Authorize("app_client_errors.create")]
        public async Task<ActionResult<AppClientErrorModel>> Create(
            [FromBody]AppClientErrorModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create app_client_errors.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 客户端错误记录 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        // [Authorize("app_client_errors.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppClientErrorModel>>> GetAll(
            [FromQuery]AppClientErrorSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all app_client_errors.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 客户端错误记录
        /// </summary>
        /// <response code="200">返回 客户端错误记录 信息</response>
        /// <response code="404"> 客户端错误记录 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        // [Authorize("app_client_errors.read")]
        public async Task<ActionResult<AppClientErrorModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get app_client_errors with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
