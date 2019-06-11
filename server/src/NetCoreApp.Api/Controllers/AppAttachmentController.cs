using System;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>附件表服务接口</summary>
    [Route("api/attachments")]
    [ApiController]
    public class AppAttachmentController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppAttachmentService service;

        public AppAttachmentController(IAppAttachmentService service) {
            this.service = service;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                service = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 附件表  </summary>
        /// <response code="200">创建 附件表 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult<AppAttachmentModel>> Create(
            [FromBody]AppAttachmentModel model
        ) {
            try {
                await service.CreateAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create app_attachments.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 附件表 </summary>
        /// <response code="204">删除 附件表 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete(string id) {
            try {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error("Can not delete app_attachments.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 附件表 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        public async Task<ActionResult<PaginatedResponseModel<AppAttachmentModel>>> GetAll(
            [FromQuery]AppAttachmentSearchModel model
        ) {
            try {
                var result = await service.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all app_attachmentss.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 附件表
        /// </summary>
        /// <response code="200">返回 附件表 信息</response>
        /// <response code="404"> 附件表 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppAttachmentModel>> GetById(string id) {
            try {
                var result = await service.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get app_attachments with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 附件表
        /// </summary>
        /// <response code="200">更新成功，返回 附件表 信息</response>
        /// <response code="404"> 附件表 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        public async Task<ActionResult<AppAttachmentModel>> Update(
            [FromRoute]string id,
            [FromBody]AppAttachmentModel model
        ) {
            try {
                var modelInDb = await service.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await service.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update app_attachments with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
