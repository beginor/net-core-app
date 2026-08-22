# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录是 ASP.NET Core 入口项目，负责装配整个应用：加载配置、注册服务、构建 HTTP 请求管线。

## 启动流程

- `Program.cs` 创建 `WebApplication`，从 `config/` 加载 `appsettings*.json`、`hibernate.config`、`log.config`；日志用 log4net（`AddLog4net`）；读取 `kestrel` section 配置服务器选项；然后 `new Startup(config, env)`，依次调用 `ConfigureServices` 与 `Configure`。
- `Startup.cs` 是 partial 主类，`ConfigureServices`/`Configure` 两个方法按固定顺序调用一组 `ConfigureXxxServices`/`ConfigureXxx` 方法，每个 `Startup*.cs` 负责一个横切面：
  - `Hibernate`：从 `config/hibernate.config` 配置 NHibernate，`AddAttributeMappingAssembly` 用 `HbmSerializer` 序列化实体程序集特性映射；开发环境开启 `ShowSql`/`FormatSql`。
  - `App`：`AddCommon(config, env)`（绑定选项、创建缓存/存储目录）、`AddData()`（自动注册仓储）；设 `Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true` 让 Dapper 匹配下划线列名。
  - `Identity`：`AddIdentity<AppUserEntity, AppRoleEntity>` + `AddHibernateStores()`，密码/锁定策略从 config 的 `identity` section 绑定。
  - `AutoMapper`：注册 `ModelMapping` Profile。
  - `Cors`/`PathBase`/`ForwardedHeaders`/`CustomHeader`/`StaticFiles`：对应中间件与响应头。
  - `OpenApi`：仅开发环境 `AddOpenApi()` + `MapOpenApi()`。
  - `Middleware`/`Routing`/`Authentication`/`Mvc`：注册管线与 MVC。
  - `WeChat`：当前注释未启用。

## 关键约定

- **MVC 显式加载 controller 程序集**：`ConfigureMvcServices` 用 `ApplicationParts.Clear()` 后只加入 `Api` 程序集（`AccountController` 所在程序集），因此控制器只在 `src/Api/Controllers`；新增 controller 程序集需在此注册。
- **配置分层**：`appsettings.json` + `appsettings.{EnvironmentName}.json` + 环境变量 + 命令行；`ASPNETCORE_PATHBASE` 默认 `/net-core-app`（见 `Properties/launchSettings.json`，监听 `http://+:5050`）。
- **路由**：`AddRouting` 开启 `LowercaseUrls = true`。
- **管道顺序**：`ConfigureServices` 与 `Configure` 的调用顺序对应且敏感（如中间件在认证之后、MVC 之前），调整时保持现有次序。

## 开发约定

- 每个横切面新增一个 `Startup.<Topic>.cs` partial 文件，内含 `ConfigureXxxServices` 与 `ConfigureXxx` 两个方法，并在 `Startup.cs` 的对应阶段按顺序调用。
- 配置项从 config section 绑定为强类型选项，用 `services.Configure<T>(section)` 或 `GetSection().Bind()`。
- 改动尽量局部：优先新增 `Startup.*.cs` 而非修改已有文件。
