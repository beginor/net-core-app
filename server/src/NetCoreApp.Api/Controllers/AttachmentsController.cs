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

    }

}
