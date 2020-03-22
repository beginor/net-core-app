using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>导航节点（菜单）服务接口</summary>
    [Route("api/app-nav-items")]
    [ApiController]
    public class AppNavItemController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppNavItemRepository repository;

        public AppNavItemController(IAppNavItemRepository repository) {
            this.repository = repository;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                repository = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 导航节点（菜单）  </summary>
        /// <response code="200">创建 导航节点（菜单） 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        [Authorize("app_nav_items.create")]
        public async Task<ActionResult<AppNavItemModel>> Create(
            [FromBody]AppNavItemModel model
        ) {
            try {
                await repository.SaveAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create app_nav_items.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 导航节点（菜单） </summary>
        /// <response code="204">删除 导航节点（菜单） 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        [Authorize("app_nav_items.delete")]
        public async Task<ActionResult> Delete(long id) {
            try {
                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error("Can not delete app_nav_items.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 导航节点（菜单） ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        [Authorize("app_nav_items.read")]
        public async Task<ActionResult<PaginatedResponseModel<AppNavItemModel>>> GetAll(
            [FromQuery]AppNavItemSearchModel model
        ) {
            try {
                var result = await repository.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all app_nav_items.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 导航节点（菜单）
        /// </summary>
        /// <response code="200">返回 导航节点（菜单） 信息</response>
        /// <response code="404"> 导航节点（菜单） 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        [Authorize("app_nav_items.read")]
        public async Task<ActionResult<AppNavItemModel>> GetById(long id) {
            try {
                var result = await repository.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get app_nav_items with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 导航节点（菜单）
        /// </summary>
        /// <response code="200">更新成功，返回 导航节点（菜单） 信息</response>
        /// <response code="404"> 导航节点（菜单） 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        [Authorize("app_nav_items.update")]
        public async Task<ActionResult<AppNavItemModel>> Update(
            [FromRoute]long id,
            [FromBody]AppNavItemModel model
        ) {
            try {
                var modelInDb = await repository.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                model.Id = id.ToString();
                await repository.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update app_nav_items with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}
