# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 常用命令

- 还原依赖：`dotnet restore NetCoreApp.slnx`
- 编译解决方案：`dotnet build NetCoreApp.slnx`
- 运行后端入口项目：`cd src/Entry && dotnet run`
  - `src/Entry/Properties/launchSettings.json` 默认监听 `http://+:5050`，并设置 `ASPNETCORE_PATHBASE=/net-core-app`。
- 运行全部测试：`cd test/Test && dotnet test`
- 运行单个测试类或方法：`cd test/Test && dotnet test --filter "FullyQualifiedName~Beginor.NetCoreApp.Test.Api.UsersControllerTest"`
  - 也可以用较短过滤词：`dotnet test --filter FullyQualifiedName~UsersControllerTest`。
- 本仓库未定义独立 lint 脚本；样式主要由 `.editorconfig` 约束，日常验证以 `dotnet build NetCoreApp.slnx` 和相关测试为主。
- 根据数据库表生成代码：先按业务需要建表，再从仓库根目录运行 `./smartcode.sh`；详细流程在 `docs/00_下载和使用.md`。

## 高层架构

这个仓库是后台服务端代码库；前端在独立仓库 `net-core-app-client`。当前项目文件目标框架是 `net10.0`，解决方案文件是 `NetCoreApp.slnx`。

主要项目：

- `src/Entry`：ASP.NET Core Web 入口项目。`Program.cs` 创建 `WebApplication`，从 `src/Entry/config/` 加载 `appsettings*.json`、`hibernate.config`、`log.config`，再调用 `Startup`。
- `src/Entry/Startup*.cs`：启动配置被拆成多个 partial 文件，分别配置 Hibernate、AutoMapper、应用服务、Identity、CORS、PathBase、ForwardedHeaders、自定义响应头、静态文件、OpenAPI、中间件、路由、认证和 MVC。
- `src/Api`：HTTP API 层，控制器位于 `Controllers/`，认证授权相关代码位于 `Authorization/` 和 `Middlewares/`。MVC 通过 `Startup.Mvc.cs` 显式加载 `Api` 程序集中的 controller。
- `src/Data`：数据访问层，实体位于 `Entities/`，仓储接口与实现位于 `Repositories/`。实体使用 `NHibernate.Mapping.Attributes` 映射；仓储通常继承 `HibernateRepository<...>`；`ServiceCollectionExtensions.AddData()` 会按名称约定自动注册以 `Repository` 结尾的服务。
- `src/Data/ModelMapping.cs`：AutoMapper Profile，维护实体与 `src/Models` DTO 之间的映射。
- `src/Models`：请求、响应、搜索条件和 DTO 模型。
- `src/Common`：共享选项、常量、扩展方法、文件/路径/验证码等帮助类。
- `src/WeChat`：微信相关功能模块；当前 `Startup.cs` 中的 WeChat 服务和管线调用处于注释状态，MVC 也未加载 WeChat controller 程序集。
- `test/Test`：NUnit 测试项目。`BaseTest` 会创建与应用相同的 DI 容器并调用 `Startup.ConfigureServices`，测试配置文件从 `src/Entry/config` 复制到测试输出目录。

## 数据库与配置

- 默认数据库是 PostgreSQL，建库脚本位于 `database/`，按编号顺序执行。
- NHibernate 连接串和方言配置位于 `src/Entry/config/hibernate.config`。
- 包版本集中管理在 `Directory.Packages.props`，各项目的共享包引用集中在 `src/package-references.props`。
- 许多测试会通过 NHibernate/Dapper 访问真实配置的数据库；运行这类测试前需要本地 PostgreSQL 与 `database/` 脚本初始化后的 schema 可用。

## 编辑约定

- `.editorconfig` 要求 UTF-8、LF、末尾换行；C# 使用 4 空格缩进，Markdown/JSON 使用 2 空格缩进。
- C# 花括号当前风格是不在成员或块声明前单独换行；`System.*` using 排在前面。
- 新增业务功能时，通常同时涉及 `src/Models` DTO、`src/Data/Entities` 实体、`src/Data/Repositories` 仓储、`src/Data/ModelMapping.cs` 映射、`src/Api/Controllers` 控制器，以及 `test/Test` 中对应测试。

## 命名规范

### 数据库
- 数据库名、表名、字段名均使用小写字母加下划线分割（`snake_case`）；
- 表名使用英文复数形式，表示集合（如 `users`、`order_items`）；
- 主键统一命名为 `id`；
- 外键格式为 `表名单数形式_字段名_id`（如 `user_id`、`order_item_id`）；

### C#
- 类名使用 PascalCase（如 `UserController`、`OrderItem`）；
- 控制器路由前缀与表名保持一致，使用复数形式，用中划线代替下划线（如 `/users`、`/order-items`）；
