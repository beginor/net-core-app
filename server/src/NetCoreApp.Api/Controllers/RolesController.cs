using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IRoleService service;

        public RolesController(IRoleService service) {
            this.service = service;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                service = null;
            }
            base.Dispose(disposing);
        }

        [HttpGet("")]
        public async Task<ActionResult<IList<ApplicationRoleModel>>> GetAll() {
            try {
                var roles = await service.GetAllAsync();
                return roles.ToList();
            }
            catch (Exception ex) {
                logger.Error("Can not get all roles!", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationRoleModel>> GetById(
            string id
        ) {
            try {
                var model = await service.GetByIdAsync(id);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get role by id {id}", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpPost("")]
        public async Task<ActionResult<ApplicationRoleModel>> Create(
            [FromBody]ApplicationRoleModel model
        ) {
            try {
                var result = await service.CreateAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not create role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationRoleModel>> Update(
            [FromRoute]string id,
            [FromBody]ApplicationRoleModel role
        ) {
            try {
                var result = await service.UpdateAsync(id, role);
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not update role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id) {
            try {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error($"Can not delete role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

    }

}
