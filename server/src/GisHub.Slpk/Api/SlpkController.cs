using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
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
        private IContentTypeProvider provider;

        public SlpkController(
            ILogger<SlpkController> logger,
            ISlpkRepository repository,
            IContentTypeProvider provider
        ) {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                logger = null;
                repository = null;
                provider = null;
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

        /// <summary>获取 slpk 场景列表</summary>
        [HttpGet("~/rest/services/slpks")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetSlpkList() {
            try {
                var models = await repository.GetAllAsync();
                var slpks = models.Select(m => new { m.Id, m.Tags, m.Longitude, m.Latitude, m.Elevation } );
                return Ok(slpks);
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk scene list .");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景信息</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetSlpkInfo(long id) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "3dSceneLayer.json");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} info!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景节点信息</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeIndex(long id, string node) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "nodes", node, "3dNodeIndexDocument.json");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} node {node} index!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景节点要素</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/features/{feature}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeFeature(long id, string node, string feature) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "nodes", node, "features", feature + ".json");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} node {node} feature {feature}!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景节点坐标</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/geometries/{geometry}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeGeometry(long id, string node, string geometry) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "nodes", node, "geometries", geometry + ".bin");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} node {node} geometries {geometry}!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景节点共享资源</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/shared")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeShared(long id, string node) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "nodes", node, "shared", "sharedResource.json");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} node {node} sharedResource.json!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>获取 slpk 场景节点贴图</summary>
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/nodes/{node}/textures/{texture}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeTexture(long id, string node, string texture) {
            try {
                var directory = await repository.GetSlpkDirectoryAsync(id);
                if (string.IsNullOrEmpty(directory)) {
                    return NotFound();
                }
                var filePath = Path.Combine(directory, "nodes", node, "textures", texture + ".bin");
                var result = ProcessFile(filePath);
                return result;
            }
            catch (Exception ex) {
                logger.LogError(ex, $"Can not get slpk {id} node {node} textures {texture}!");
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        private ActionResult ProcessFile(string filePath) {
            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists) {
                filePath = filePath + ".gz";
                fileExists = System.IO.File.Exists(filePath);
            }
            if (!fileExists) {
                return NotFound();
            }
            var fileInfo = new FileInfo(filePath);
            var fileTime = fileInfo.LastWriteTimeUtc.ToFileTime().ToString("x");
            var etag = Request.Headers["If-None-Match"].ToString();
            if (fileTime.Equals(etag, StringComparison.OrdinalIgnoreCase)) {
                return StatusCode(StatusCodes.Status304NotModified);
            }
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["ETag"] = fileTime;
            var fileName = fileInfo.Name;
            string contentType = string.Empty;
            if (fileName.EndsWith(".gz")) {
                Response.Headers["Content-Encoding"] = "gzip";
                provider.TryGetContentType(
                    fileName.Substring(0, fileName.Length - 3),
                    out contentType
                );
            }
            else {
                provider.TryGetContentType(fileName, out contentType);
            }
            return File(fileInfo.OpenRead(), contentType);
        }

    }

}
