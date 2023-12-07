using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using NHibernate.AspNetCore.Identity;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Api.Controllers;

/// <summary>角色 API</summary>
[Route("api/roles")]
[ApiController]
public class RolesController : Controller {

    private ILogger<RolesController> logger;
    private RoleManager<AppRole> roleMgr;
    private UserManager<AppUser> userMgr;
    private IIdentityRepository identityRepo;
    private IMapper mapper;
    private IAppOrganizeUnitRepository orgUnitRepo;

    public RolesController(
        ILogger<RolesController> logger,
        RoleManager<AppRole> roleMgr,
        UserManager<AppUser> userMgr,
        IIdentityRepository identityRepo,
        IMapper mapper,
        IAppOrganizeUnitRepository orgUnitRepo
    ) {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.roleMgr = roleMgr ?? throw new ArgumentNullException(nameof(roleMgr));
        this.userMgr = userMgr ?? throw new ArgumentNullException(nameof(userMgr));
        this.identityRepo = identityRepo ?? throw new ArgumentNullException(nameof(identityRepo));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.orgUnitRepo = orgUnitRepo ?? throw new ArgumentNullException(nameof(orgUnitRepo));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // disable managed resource here;
        }
        base.Dispose(disposing);
    }

    /// <summary>创建角色</summary>
    /// <response code="200">创建角色成功并返回角色信息</response>
    /// <response code="400">创建角色失败并返回错误信息</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("")]
    [Authorize("app_roles.create")]
    public async Task<ActionResult<AppRoleModel>> Create(
        [FromBody]AppRoleModel model
    ) {
        try {
            if (await roleMgr.RoleExistsAsync(model.Name!)) {
                return BadRequest($"Role already {model.Name} exists!");
            }
            var role = mapper.Map<AppRole>(model);
            var result = await roleMgr.CreateAsync(role);
            if (result.Succeeded) {
                mapper.Map(role, model);
                return model;
            }
            return BadRequest(result.GetErrorsString());
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not create role with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除指定的角色</summary>
    /// <response code="204">删除角色成功</response>
    /// <response code="400">删除角色出错并返回错误信息</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [Authorize("app_roles.delete")]
    public async Task<ActionResult> Delete(long id) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NoContent();
            }
            var result = await roleMgr.DeleteAsync(role);
            if (result.Succeeded) {
                return NoContent();
            }
            return BadRequest(result.GetErrorsString());
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not delete role by id {id}");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>获取角色列表</summary>
    /// <response code="200">获取成功并返回角色列表。</response>
    /// <response code="500">服务器内部错误。</response>
    [HttpGet("")]
    [Authorize("app_roles.read")]
    public async Task<ActionResult<PaginatedResponseModel<AppRoleModel>>> GetAll(
        [FromQuery]AppRoleSearchModel model
    ) {
        try {
            var currUser = await userMgr.FindByNameAsync(User.Identity!.Name!);
            var userUnit = currUser!.OrganizeUnit!;
            var unitCode = userUnit.Code;
            if (model.OrganizeUnitId.HasValue) {
                var unitId = model.OrganizeUnitId.Value;
                var canView = await orgUnitRepo.CanViewOrganizeUnitAsync(userUnit.Id, unitId);
                if (!canView) {
                    return BadRequest($"Can not view organize unit {unitId}");
                }
                var unit = await orgUnitRepo.GetEntityByIdAsync(unitId);
                unitCode = unit.Code;
            }
            model.OrganizeUnitCode = unitCode;
            var data = await identityRepo.SearchAsync(model);
            var result = new PaginatedResponseModel<AppRoleModel> {
                Skip = model.Skip,
                Take = model.Take,
                Total = data.Count,
                Data = data
            };
            return result;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not search roles with {model.ToJson()}");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>获取指定角色</summary>
    /// <response code="404">找不到指定的角色。</response>
    /// <response code="200">获取角色成功，返回角色信息。</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}")]
    [Authorize("app_roles.read_by_id")]
    public async Task<ActionResult<AppRoleModel>> GetById(
        long id
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            var model = mapper.Map<AppRoleModel>(role);
            return model;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get role by id {id}");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>更新指定的角色</summary>
    /// <response code="200">更新成功并返回角色信息</response>
    /// <response code="400">更新角色出错并返回错误信息</response>
    /// <response code="404">指定的角色不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id:long}")]
    [Authorize("app_roles.update")]
    public async Task<ActionResult<AppRoleModel>> Update(
        [FromRoute]long id,
        [FromBody]AppRoleModel model
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            mapper.Map(model, role);
            var result = await roleMgr.UpdateAsync(role);
            if (result.Succeeded) {
                mapper.Map(role, model);
                return model;
            }
            return BadRequest(result.GetErrorsString());
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not update role by id {id} with {model.ToJson()} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>获取指定角色下的的用户</summary>
    /// <response code="200">获取成功，返回该角色的用户列表</response>
    /// <response code="404">指定的角色不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}/users")]
    [Authorize("app_roles.read_users_in_role")]
    public async Task<ActionResult<IList<AppUserModel>>> GetUsersInRole(
        long id
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            var users = await userMgr.GetUsersInRoleAsync(role.Name!);
            var models = mapper.Map<IList<AppUserModel>>(users);
            return models.ToList();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get users in role {id}");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>读取指定角色的权限</summary>
    /// <response code="200">获取成功，返回该角色的权限列表</response>
    /// <response code="404">指定的角色不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id:long}/privileges")]
    [Authorize("app_roles.read_role_privileges")]
    public async Task<ActionResult<IList<string>>> GetPrivilegesInRole(
        long id
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            var claims = await roleMgr.GetClaimsAsync(role);
            var privileges = claims.Where(c => c.Type == Consts.PrivilegeClaimType)
                .Select(c => c.Value)
                .ToList();
            return privileges;
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not get privileges in role {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>添加权限至角色</summary>
    /// <response code="200">添加成功。</response>
    /// <response code="404">指定的角色不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost("{id:long}/privileges/{privilege}")]
    [Authorize("app_roles.add_privilege_to_role")]
    public async Task<ActionResult> AddPrivilegeToRole(
        long id,
        string privilege
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            var claims = await roleMgr.GetClaimsAsync(role);
            if (!claims.Any(c => c.Type == Consts.PrivilegeClaimType && c.Value == privilege)) {
                await roleMgr.AddClaimAsync(role, new Claim(Consts.PrivilegeClaimType, privilege));
            }
            return Ok();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not add privilege {privilege} to role {id} .");
            return this.InternalServerError(ex);
        }
    }

    /// <summary>删除角色的权限</summary>
    /// <response code="204">删除成功。</response>
    /// <response code="404">指定的角色不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id:long}/privileges/{privilege}")]
    [Authorize("app_roles.remove_privilige_from_role")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> RemovePrivilegeFromRole(
        long id,
        string privilege
    ) {
        try {
            var role = await roleMgr.FindByIdAsync(id.ToString());
            if (role == null) {
                return NotFound();
            }
            var claims = await roleMgr.GetClaimsAsync(role);
            var claim = claims.FirstOrDefault(c => c.Type == Consts.PrivilegeClaimType && c.Value == privilege);
            if (claim != null) {
                await roleMgr.RemoveClaimAsync(role, claim);
            }
            return NoContent();
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Can not remove privilege {privilege} from role {id}.");
            return this.InternalServerError(ex);
        }
    }

}
