using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Beginor.AppFx.Core;
using Beginor.AppFx.Api;

namespace Beginor.GisHub.Slpk.Api {

    partial class SlpkController {

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
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/layers/0/nodes/{node}")]
        [Authorize("slpks.read_slpk_scene")]
        public Task<ActionResult> GetLayer0NodeIndex(long id, string node) {
            return GetNodeIndex(id, node);
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
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/layers/0/nodes/{node}/features/{feature}")]
        [Authorize("slpks.read_slpk_scene")]
        public Task<ActionResult> GetLayer0NodeFeature(long id, string node, string feature) {
            return GetNodeFeature(id, node, feature);
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
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/layers/0/nodes/{node}/geometries/{geometry}")]
        [Authorize("slpks.read_slpk_scene")]
        public Task<ActionResult> GetLayer0NodeGeometry(long id, string node, string geometry) {
            return GetNodeGeometry(id, node, geometry);
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
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/layers/0/nodes/{node}/shared")]
        [Authorize("slpks.read_slpk_scene")]
        public Task<ActionResult> GetLayer0NodeShared(long id, string node) {
            return GetNodeShared(id, node);
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
        [HttpGet("~/rest/services/slpks/{id:long}/SceneServer/layers/0/nodes/{node}/textures/{texture}")]
        [Authorize("slpks.read_slpk_scene")]
        public Task<ActionResult> GetLayer0NodeTexture(long id, string node, string texture) {
            return GetNodeTexture(id, node, texture);
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
