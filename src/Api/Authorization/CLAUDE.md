# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含认证与授权相关代码，实现基于权限（Privilege）的动态授权，不依赖固定的策略配置。

## 核心机制

- `AuthorizationPolicyProvider` 继承 `DefaultAuthorizationPolicyProvider`，重写 `GetPolicyAsync`：当静态策略不存在时，动态构造一个策略——要求已认证用户且拥有 `AppClaimTypes.Privilege` 等于策略名的声明。因此控制器里 `[Authorize("app_users.read")]` 的策略名本身就是权限值，无需在启动代码里逐一注册策略。
- 认证支持双重 scheme：JWT Bearer（`JwtBearerDefaults.AuthenticationScheme`）与自定义 Token 认证（`TokenOptions.DefaultSchemaName`，来自 `Beginor.AspNetCore.Authentication.Token`）。动态策略用 `AddAuthenticationSchemes(...)` 同时启用两者。
- `JwtBearerEventsHandler` / `TokenEventsHandler` 处理令牌事件（如从请求提取令牌、写入响应）。
- `DistributedCacheExtensions` 提供用户声明的分布式缓存读写，配合控制器把角色/权限缓存起来。

## 开发约定

- 新增权限点只需在控制器 action 上加 `[Authorize("privilege_name")]`，并在数据库 `app_privileges` 表与角色声明中维护对应值，无需改动本目录代码。
- 权限声明类型常量集中在 `src/Common/AppClaimTypes.cs`（如 `Privilege`、`OrganizeUnitId`），新增声明类型在此定义。
- 令牌相关配置（密钥、过期时间）在 `src/Common/JwtOption.cs`，由 config 的 `jwt` section 绑定。
