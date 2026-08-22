# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含 ASP.NET Core 中间件，处理全局异常、访问审计日志与 Referer 过滤。中间件在 `Startup.Middleware.cs` 中按顺序注册。

## 中间件清单

- `ExceptionMiddleware`：最外层异常拦截。包裹后续管线，捕获未处理异常，记录日志并返回 500；开发环境回显完整异常，生产环境只返回通用提示。
- `AuditLogMiddleware`：继承 `Beginor.AppFx.Core` 的 `Disposable`，记录每次请求的审计日志。实现要点：
  - 用 `System.Threading.Channels.Channel<AppAuditLogEntity>` 收集日志，后台 `Task.Run(StartSaveAuditLog)` 消费；
  - 批量（默认 128）通过 `ISessionFactory.OpenStatelessSession()` + Dapper `ExecuteAsync` 写入数据库，`EntityHelper.GenerateInsertSql` 生成插入 SQL；
  - 通过 `IActionSelector` + 路由模板匹配出 `ControllerActionDescriptor`，回填 `ControllerName`/`ActionName`；
  - 从 `X-Real-IP` 或连接远程地址取 IP。
- `RefererFilteringMiddleware`：依据 CORS 策略的 `Origins` 校验 Referer，不匹配返回 400；提供扩展方法 `UseRefererFiltering`。当前未在 `ConfigureMiddleware` 中注册。

## 注册与顺序

```csharp
// Startup.Middleware.cs
app.UseMiddleware<ExceptionMiddleware>();   // 必须最先，保证捕获全部异常
app.UseMiddleware<AuditLogMiddleware>();    // 其后，记录请求审计
```

- 顺序敏感：`ExceptionMiddleware` 必须在最前（最外层）才能捕获后续所有异常；`AuditLogMiddleware` 依赖它之后的请求结果状态码。

## 开发约定

- 中间件用 primary constructor 注入依赖，对必需依赖做 `ArgumentNullException` 校验（参考现有写法）。
- 需要优雅关闭/取消的中间件继承 `Disposable`，在 `Dispose` 中取消后台任务（`cts.Cancel()`）。
- 后台写入用 Channel + 批处理事务，避免逐条写库；新增全局横切逻辑优先考虑中间件而非控制器。
- 新增中间件后在 `ConfigureMiddleware` 中按顺序 `app.UseMiddleware<...>()` 注册。
