# Claude Code 指南

这个文件为处理这个仓库中的代码时， 为 Claude Code (claude.ai/code) 提供指导。

## 命令

- 还原依赖: `dotnet restore NetCoreApp.slnx`
- 编译解决方案: `dotnet build NetCoreApp.slnx`
- 运行入口项目: `cd src/Entry && dotnet run`
- 运行单元测试: `cd test/Test && dotnet test --filter FullyQualifiedName~{TestFilter}`

## 事实

这个代码库只是后台服务端代码库， 前端代码在单独的仓库 `net-core-app-client` 中；

- 主要的层次结构：
  - `src/Entry` 是程序的入口， 配置文件从 `src/Entry/config/` 加载， 启动时 `Startup` 分为多个文件。
  - `src/Api` (HTTP/认证/中间件)；
  - `src/Data` (NHibernate 仓储/实体, 以及 Dapper 进行 SQL 查询，使用 NHibernate.Mapping.Attributes 来进行实体关系映射)；
  - `src/Models` (DTOs)；
  - `src/Common` (共享的帮助类/选项)；
  - `test/Test` (NUnit 测试)；
- 数据库是 PostgreSQL， 通过 `src/Entry/config/hibernate.config` 配置；
