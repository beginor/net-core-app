using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.GisHub.TileMap.Models;
using Beginor.GisHub.TileMap.Data;

namespace Beginor.GisHub.TileMap.Api {

    /// <summary>矢量切片包 服务接口</summary>
    [ApiController]
    [Route("api/vectortiles")]
    public partial class VectortileController : Controller {

        private ILogger<VectortileController> logger;
        private IVectortileRepository repository;

        public VectortileController(
            ILogger<VectortileController> logger,
            IVectortileRepository repository
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

        /// <summary>搜索 矢量切片包 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("vectortiles.read")]
        public async Task<ActionResult<PaginatedResponseModel<VectortileModel>>> Search(
            [FromQuery]VectortileSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search vectortiles with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary> 创建 矢量切片包 </summary>
        /// <response code="200">创建 矢量切片包 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("vectortiles.create")]
        public async Task<ActionResult<VectortileModel>> Create(
            [FromBody]VectortileModel model
        ) {
            try {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                await repository.SaveAsync(model, userId);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to vectortiles.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 矢量切片包 </summary>
        /// <response code="204">删除 矢量切片包 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("vectortiles.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                await repository.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete vectortiles by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 矢量切片包
        /// </summary>
        /// <response code="200">返回 矢量切片包 信息</response>
        /// <response code="404"> 矢量切片包 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("vectortiles.read")]
        public async Task<ActionResult<VectortileModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get vectortiles by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 矢量切片包
        /// </summary>
        /// <response code="200">更新成功，返回 矢量切片包 信息</response>
        /// <response code="404"> 矢量切片包 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("vectortiles.update")]
        public async Task<ActionResult<VectortileModel>> Update(
            [FromRoute]long id,
            [FromBody]VectortileModel model
        ) {
            try {
                var exists = await repository.ExitsAsync(id);
                if (!exists) {
                    return NotFound();
                }
                var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                await repository.UpdateAsync(id, model, userId);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update vectortiles by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
