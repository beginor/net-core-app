using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>附件表服务接口</summary>
    [Route("api/attachments")]
    [ApiController]
    public class AppAttachmentController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppAttachmentRepository repository;

        public AppAttachmentController(IAppAttachmentRepository repository) {
            this.repository = repository;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 附件表  </summary>
        /// <response code="200">创建 附件表 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("app_attachments.create")]
        public async Task<ActionResult<AppAttachmentModel>> Create(
            [FromBody]AppAttachmentModel model
        ) {
            try {
                await repository.SaveAsync(model);
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
        [Authorize("app_attachments.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
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
        [Authorize("app_attachments.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppAttachmentModel>>> GetAll(
            [FromQuery]AppAttachmentSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
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
        [Authorize("app_attachments.read")]
        public async Task<ActionResult<AppAttachmentModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
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
        [Authorize("app_attachments.update")]
        public async Task<ActionResult<AppAttachmentModel>> Update(
            [FromRoute]long id,
            [FromBody]AppAttachmentModel model
        ) {
            try {
                var modelInDb = await repository.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                model.Id = id.ToString();
                await repository.UpdateAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update app_attachments with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
