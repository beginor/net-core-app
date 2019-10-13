using System;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>审计日志服务接口</summary>
    [Route("api/app-audit-logs")]
    [ApiController]
    public class AppAuditLogController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppAuditLogRepository repository;

        public AppAuditLogController(IAppAuditLogRepository repository) {
            this.repository = repository;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 审计日志  </summary>
        /// <response code="200">创建 审计日志 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize(Policy = "app_audit_logs.create")]
        public async Task<ActionResult<AppAuditLogModel>> Create(
            [FromBody]AppAuditLogModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create app_audit_logs.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 审计日志 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize(Policy = "app_audit_logs.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppAuditLogModel>>> GetAll(
            [FromQuery]AppAuditLogSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all app_audit_logs.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 审计日志
        /// </summary>
        /// <response code="200">返回 审计日志 信息</response>
        /// <response code="404"> 审计日志 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize(Policy = "app_audit_logs.read")]
        public async Task<ActionResult<AppAuditLogModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get app_audit_logs with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
