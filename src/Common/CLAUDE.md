# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含跨层共享的选项类、常量、扩展方法与帮助类，被 `Api`、`Data`、`Entry` 等多个项目引用。

## 选项模式（Options）

- 配置通过强类型选项类从 config section 绑定，注册在 `ServiceCollectionExtensions.AddCommon(config, env)` 中完成：
  - `common` section → `CommonOption`（含 `CacheOption`、`StorageOption`）；
  - `attachment` → `AppAttachmentOptions`；
  - `captcha` → `CaptchaOptions`。
- 选项类是简单 POCO，带默认值；`AddCommon` 启动时会按选项在 `ContentRootPath` 下创建 `cache`/`storage` 目录（不存在时）。
- `JwtOption` 也在此定义，从 `jwt` section 绑定，供 `AccountController` 生成令牌。

## 帮助类与扩展

- 文件/路径：`FileHelper`、`PathHelper`。
- 实体：`EntityHelper`（实体相关辅助）。
- 验证码：`CaptchaGenerator` 实现 `ICaptchaGenerator`，`ValidateCodeAsync` 校验。
- 安全：`SafeUrlEncoder` 对 URL 中的用户名/密码做编解码。
- 扩展方法：`ClaimsPrincipalExtensions`（`GetUserId()` 等，控制器用来取登录用户 id）、`DistributedCacheExtensions`（用户声明缓存）。

## 常量与格式

- `Consts`：全局常量（如临时令牌 query 键 `TmpToken`）。
- `AppClaimTypes`：自定义声明类型（`Privilege`、`OrganizeUnitId`、`OrganizeUnitCode`）。
- `CacheKeyFormat`：分布式缓存 key 的格式化模板（如 `TmpToken`）。

## 开发约定

- 新增全局配置项时，在此加对应选项类并在 `AddCommon` 里 bind + `AddSingleton`，保持与现有风格一致。
- 跨项目共享的帮助类优先放本目录而非某个业务层；只被单层使用的辅助代码留在该层内。
