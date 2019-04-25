using System;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>附件 API</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppAttachmentService attachmentSvc;

        public AttachmentsController(IAppAttachmentService attachmentSvc) {
            this.attachmentSvc = attachmentSvc;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                attachmentSvc = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建附件 </summary>
        /// <response code="200">创建附件成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult<AppAttachmentModel>> Create(
            [FromBody]AppAttachmentModel model
        ) {
            try {
                await attachmentSvc.CreateAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create attachment.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>删除附件</summary>
        /// <response code="204">删除附件成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete(string id) {
            try {
                await attachmentSvc.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error("Can not delete attachment.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>获取全部附件</summary>
        [HttpGet("")]
        public async Task<ActionResult<PaginatedResponseModel<AppAttachmentModel>>> GetAll(
            [FromQuery]AppAttachmentSearchModel model
        ) {
            try {
                var result = await attachmentSvc.Search(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all attachments.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的附件
        /// </summary>
        /// <response code="200">返回附件信息</response>
        /// <response code="404">附件不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppAttachmentModel>> GetById(string id) {
            try {
                var result = await attachmentSvc.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get attachment with {id}.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新附件
        /// </summary>
        /// <response code="200">更新成功，返回附件信息</response>
        /// <response code="404">附件不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<AppAttachmentModel>> Update(
            [FromRoute]string id,
            [FromBody]AppAttachmentModel model
        ) {
            try {
                var modelInDb = await attachmentSvc.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await attachmentSvc.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update attachment with {id}.", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

    }

}
