using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.AspNetCore.Identity;

namespace Beginor.NetCoreApp.Api.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private RoleManager<ApplicationRole> manager;

        public RolesController(RoleManager<ApplicationRole> manager) {
            this.manager = manager;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                manager = null;
            }
            base.Dispose(disposing);
        }

        [HttpPost("")]
        public async Task<ActionResult<ApplicationRoleModel>> Create(
            [FromBody]ApplicationRoleModel model
        ) {
            try {
                if (await manager.RoleExistsAsync(model.Name)) {
                    return BadRequest($"Role already {model.Name} exists!");
                }
                var role = Mapper.Map<ApplicationRole>(model);
                var result = await manager.CreateAsync(role);
                if (result.Succeeded) {
                    Mapper.Map(role, model);
                    return model;
                }
                return StatusCode(406, result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not create role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id) {
            try {
                var role = await manager.FindByIdAsync(id);
                if (role == null) {
                    return NoContent();
                }
                var result = await manager.DeleteAsync(role);
                if (result.Succeeded) {
                    return NoContent();
                }
                return StatusCode(406, result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not delete role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpGet("")]
        public ActionResult<IList<ApplicationRoleModel>> GetAll() {
            try {
                var roles = manager.Roles.ToList();
                var models = Mapper.Map<IList<ApplicationRoleModel>>(roles);
                return models.ToList();
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
                var role = await manager.FindByIdAsync(id);
                if (role == null) {
                    return NotFound();
                }
                var model = Mapper.Map<ApplicationRoleModel>(role);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not get role by id {id}", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationRoleModel>> Update(
            [FromRoute]string id,
            [FromBody]ApplicationRoleModel model
        ) {
            try {
                var role = await manager.FindByIdAsync(id);
                if (role == null) {
                    return NotFound();
                }
                Mapper.Map(model, role);
                var result = await manager.UpdateAsync(role);
                if (result.Succeeded) {
                    Mapper.Map(role, model);
                    return model;
                }
                return StatusCode(406, result.GetErrorsString());
            }
            catch (Exception ex) {
                logger.Error($"Can not update role", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

    }

}
