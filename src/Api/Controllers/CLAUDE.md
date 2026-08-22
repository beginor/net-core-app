# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含所有 MVC 控制器。控制器负责接收请求、调用仓储/管理器处理业务、返回模型（`src/Models`），不做持久化以外的逻辑。

## 控制器结构

- 每个控制器用 primary constructor 注入依赖：`ILogger<...>`、`UserManager<AppUserEntity>`、`RoleManager<AppRoleEntity>`、`IMapper`、仓储接口（`I...Repository`）等。
- 类标记 `[ApiController]`，路由前缀与表名一致，用复数 + 中划线，统一加 `api/` 前缀（如 `UsersController` → `api/users`，`AccountController` → `api/account`）。
- 接口复杂的控制器用 partial 拆分，按业务分组放到 `Controller名.分组.cs`（如 `AccountController.menu.cs`、`AccountController.token.cs`、`AccountController.userinfo.cs`）；主文件放登录、凭证等核心方法，partial 文件只写分组内的 action 与私有方法。

## 权限与认证

- 权限精确到每个 action，用 `[Authorize("privilege_name")]` 控制；策略名即权限值，由 `AuthorizationPolicyProvider` 动态映射为 `RequireClaim(AppClaimTypes.Privilege, policyName)`。
- 需要权限的 action 必须显式加 `[Authorize]`；无需鉴权的 action（如登录、验证码、匿名用户信息）不加。
- 用扩展方法 `this.GetUserId()` 获取登录用户 id（字符串，需自行 `long.TryParse`）。

## 数据操作约定

- 用户/角色/权限等身份数据通过 `UserManager`/`RoleManager` 操作，不直接写仓储；组织单元、导航菜单等用对应仓储。
- 每个 action 用 `try/catch`，catch 中 `logger.LogError(ex, $"...")` 并 `return this.InternalServerError(ex)`；业务校验失败返回 `BadRequest(...)` / `NotFound()` / `Forbid()`。
- 来自 URL/请求体的敏感输入（用户名、密码等）用 `SafeUrlEncoder.Decode(...)` 解码后再使用。
- 日志中拼接对象用 `model.ToJson()`。

## 返回约定

- 查询返回 `ActionResult<T>` 或 `ActionResult<PaginatedResponseModel<T>>`；分页参数用 `Skip`/`Take`/`Total`。
- 创建/更新成功返回模型本身；删除成功返回 `NoContent()` 并加 `[ProducesResponseType(204)]`。
- 用 `[HttpGet]/[HttpPost]/[HttpPut]/[HttpDelete]` 显式指定动词与路由模板，id 路由约束用 `{id:long}`。
