using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [Route("")]
        public ActionResult GetAll() {
            try {
                var roles = manager.Roles.ToList();
                return Ok(roles);
            }
            catch (Exception ex) {
                logger.Error("Can not get all roles!", ex);
                return StatusCode(500, ex.GetOriginalMessage());
            }
        }

    }

}
