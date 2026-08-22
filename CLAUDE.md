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

- `src/Entry`：ASP.NET Core Web 入口项目，详见 [`src/Entry/CLAUDE.md`](src/Entry/CLAUDE.md)。
- `src/Api`：HTTP API 层，控制器在 `Controllers/`，认证授权在 `Authorization/` 和 `Middlewares/`，详见各目录 CLAUDE.md。
- `src/Data`：数据访问层，实体在 `Entities/`，仓储在 `Repositories/`，详见各目录 CLAUDE.md；`AddData()` 会按名称约定自动注册以 `Repository` 结尾的服务。
- `src/Models`：请求、响应、搜索条件和 DTO 模型，与实体的映射见 `src/Data/ModelMapping.cs`，详见 [`src/Models/CLAUDE.md`](src/Models/CLAUDE.md)。
- `src/Common`：共享选项、常量、扩展方法与帮助类，详见 [`src/Common/CLAUDE.md`](src/Common/CLAUDE.md)。
- `src/WeChat`：微信模块，当前未启用，详见 [`src/WeChat/CLAUDE.md`](src/WeChat/CLAUDE.md)。
- `test/Test`：NUnit 测试项目，复用应用 DI 容器，详见 [`test/Test/CLAUDE.md`](test/Test/CLAUDE.md)。

## 数据库与配置

- 默认数据库是 PostgreSQL，建库脚本位于 `database/`，按编号顺序执行，详见 [`database/CLAUDE.md`](database/CLAUDE.md)。
- NHibernate 连接串和方言配置位于 `src/Entry/config/hibernate.config`。
- 包版本集中管理在 `Directory.Packages.props`，各项目的共享包引用集中在 `src/package-references.props`。
- 测试通过 NHibernate/Dapper 访问真实数据库，运行前需本地 PostgreSQL 与初始化后的 schema 可用，详见 [`test/Test/CLAUDE.md`](test/Test/CLAUDE.md)。

## 编辑约定

- `.editorconfig` 要求 UTF-8、LF、末尾换行；C# 使用 4 空格缩进，Markdown/JSON 使用 2 空格缩进。
- C# 花括号当前风格是不在成员或块声明前单独换行；`System.*` using 排在前面。
- 新增业务功能时，通常同时涉及 `src/Models` DTO、`src/Data/Entities` 实体、`src/Data/Repositories` 仓储、`src/Data/ModelMapping.cs` 映射、`src/Api/Controllers` 控制器，以及 `test/Test` 中对应测试。

## 命名规范

### 数据库
数据库命名规范（`snake_case`、复数表名、`id` 主键、外键格式等）详见 [`database/CLAUDE.md`](database/CLAUDE.md)。

### C#
- 类名使用 PascalCase（如 `UserController`、`OrderItem`）；
- 控制器路由前缀与表名保持一致，使用复数形式，用中划线代替下划线（如 `/users`、`/order-items`）；
