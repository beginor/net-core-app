using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.AppFx.Api;
using Beginor.GisHub.Slpk.Data;
using Beginor.GisHub.Slpk.Models;

namespace Beginor.GisHub.Slpk.Api {

    [Route("api/slpks")]
    [ApiController]
    public class SlpkController : Controller {

        private ILogger<SlpkController> logger;
        private ISlpkRepository repository;

        public SlpkController(
            ILogger<SlpkController> logger,
            ISlpkRepository repository
        ) {
            this.logger = logger;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 slpk 航拍模型 </summary>
        /// <response code="200">创建 slpk 航拍模型 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("slpks.create")]
        public async Task<ActionResult<SlpkModel>> Create(
            [FromBody]SlpkModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not save {model.ToJson()} to slpks.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 slpk 航拍模型 </summary>
        /// <response code="204">删除 slpk 航拍模型 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("slpks.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not delete slpks by id {id} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 slpk 航拍模型 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("slpks.read")]
        public async Task<ActionResult<PaginatedResponseModel<SlpkModel>>> GetAll(
            [FromQuery]SlpkSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not search slpks with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 slpk 航拍模型
        /// </summary>
        /// <response code="200">返回 slpk 航拍模型 信息</response>
        /// <response code="404"> slpk 航拍模型 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("slpks.read")]
        public async Task<ActionResult<SlpkModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpks by id {id}.");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 slpk 航拍模型
        /// </summary>
        /// <response code="200">更新成功，返回 slpk 航拍模型 信息</response>
        /// <response code="404"> slpk 航拍模型 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("slpks.update")]
        public async Task<ActionResult<SlpkModel>> Update(
            [FromRoute]long id,
            [FromBody]SlpkModel model
        ) {
            try {
                var modelInDb = await repository.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await repository.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not update slpks by id {id} with {model.ToJson()} .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景信息</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetSlpkInfo(long id) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点信息</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeIndex(string node) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点要素</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/features/{feature}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeFeature(long id, string node, string feature) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点坐标</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/geometries/{geometry}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeGeometry(long id, string node, string geometry) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点共享资源</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/shared/{resource}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeShared(long id, string node, string resource) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点贴图</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/textures/{texture}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeTexture(long id, string node, string texture) {
            throw new NotImplementedException();
        }

    }

}
