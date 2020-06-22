using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Beginor.GisHub.Slpk.Api {

    [Route("rest/services/slpks")]
    [ApiController]
    public class SlpkController : Controller {

        private ILogger<SlpkController> logger;

        public SlpkController(
            ILogger<SlpkController> logger
        ) {
            this.logger = logger;
        }

        /// <summary>获取 slpk 场景列表</summary>
        [HttpGet("")]
        [Authorize("slpks.read_slpk_list")]
        public async Task<ActionResult> GetSlpkList() {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景信息</summary>
        [HttpGet("{id:long}/SceneServer")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetSlpkInfo(long id) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点信息</summary>
        [HttpGet("nodes/{node}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeIndex(string node) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点要素</summary>
        [HttpGet("nodes/{node}/features/{feature}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeFeature(string node, string feature) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点坐标</summary>
        [HttpGet("nodes/{node}/geometries/{geometry}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeGeometry(string node, string geometry) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点共享资源</summary>
        [HttpGet("nodes/{node}/shared/{resource}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeShared(string node, string resource) {
            throw new NotImplementedException();
        }

        /// <summary>获取 slpk 场景节点贴图</summary>
        [HttpGet("nodes/{node}/textures/{texture}")]
        [Authorize("slpks.read_slpk_scene")]
        public async Task<ActionResult> GetNodeTexture(string node, string texture) {
            throw new NotImplementedException();
        }

    }

}
