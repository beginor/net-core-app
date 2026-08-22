# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录是微信相关功能模块，当前实现的是微信小程序登录授权（`jscode2session`、手机号解密、绑定已有用户并返回临时令牌）。

> ⚠️ 该模块当前**未启用**：`Startup.WeChat.cs` 中的 `ConfigureWeChatServices`/`ConfigureWeChat` 被注释，`Startup.Mvc.cs` 中 WeChat controller 程序集也未注册。启用需同时恢复这两处。

## 目录结构

- `Common/`
  - `ApiGateway`：微信 API 网关。封装 `HttpClient`（经 `ProxyUtil.CreateHttpClient` 走代理），负责获取 `access_token`（带分布式缓存）、`RequestAsync<T>` 统一 POST/错误检查（`ErrCode != 0` 抛异常）。实现 `IDisposable` + 终结器。
  - `ApiGatewayExtensions`：扩展方法 `GetJsCode2SessionAsync`，构造 `jscode2session` 请求 URL。
  - `WeChatOption`：从 config 的 `wechat` section 绑定（`AppId`、`Secret`、`GrantType`、`BaseUrl`）。
  - `ProxyUtil`：代理/HTTP 客户端与 AES 解密工具（解密微信用户数据）。
- `Models/`：微信请求/响应模型（`WeChatRequest`、`WeChatLoginModels`、`WeChatResponse`）。
- `Api/WeChatController.cs`：`[Route("api/wechat")]`，`LoginByPhone` 处理小程序手机号登录。

## 登录流程（`WeChatController.LoginByPhone`）

1. `GetWeChatSessionAsync`：用 `code` 换取 `jscode2session`，`sessionId` 缓存 60 分钟；
2. 按 `Consts.WeChatAuth` + `OpenId` 查找已有用户；
3. 解密 `EncryptedData`/`IV` 得手机号，按手机号匹配现有用户并 `AddLoginAsync` 绑定微信登录；
4. `JwtResultAsync`：更新登录信息，生成临时令牌存入分布式缓存（`CacheKeyFormat.TmpToken`），返回 `TmpTokenKey`/`TmpTokenValue` 给客户端，客户端再用临时令牌换取正式 JWT（见 `AccountController`）。

## 开发约定

- 微信相关缓存 key 在 `src/Common/CacheKeyFormat.cs`（如 `WeChatAuthAccessToken`、`WeChatAuthSession`）；新增缓存沿用此格式。
- 微信登录标识常量在 `src/Common/Consts.cs`（`WeChatAuth`、`TmpToken`）。
- 与主站用户体系通过 `UserManager` 的登录（login）关联，不新建用户表。
- 恢复启用时：在 `Startup.WeChat.cs` 取消注释服务注册，并在 `Startup.Mvc.cs` 的 `ApplicationParts` 加入 WeChat controller 程序集。
