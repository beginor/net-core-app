# CLAUDE.md 编写总结

本文档汇总为 `net-core-app` 项目各目录编写的 CLAUDE.md 索引与核心约定，便于后续维护与查阅。

## 现有 CLAUDE.md 一览

| 层级 | 文件 | 核心内容 |
|------|------|----------|
| 根 | `CLAUDE.md` | 常用命令、高层架构、数据库与配置、编辑约定、命名规范 |
| 根 | `database/CLAUDE.md` | 数据库命名规范、表设计原则、SQL 文件编号规范、软删除约定 |
| 数据层 | `src/Data/Entities/CLAUDE.md` | 实体开发约定：`#nullable disable`、`BaseEntity<long>`、`ManyToOne`、软删除 Where、PostgreSQL 数组映射 |
| 数据层 | `src/Data/Repositories/CLAUDE.md` | 仓储接口与实现约定：`HibernateRepository` 基类、事务、审计字段、软删除、Dapper 复杂查询 |
| API 层 | `src/Api/Controllers/CLAUDE.md` | 控制器结构、partial 拆分、`[Authorize("权限名")]` 动态授权、`try/catch` 模式、`SafeUrlEncoder`、返回约定 |
| API 层 | `src/Api/Authorization/CLAUDE.md` | `AuthorizationPolicyProvider` 动态策略、JWT + Token 双 scheme、声明类型常量、新增权限点无需改代码 |
| API 层 | `src/Api/Middlewares/CLAUDE.md` | 三个中间件职责、注册顺序（Exception 最外层）、AuditLog Channel 批量写入、`Disposable` 优雅关闭 |
| 模型层 | `src/Models/CLAUDE.md` | 基类体系（`StringEntity`/`PaginatedRequestModel`/`StringIdNameEntity`）、DataAnnotations 校验、与 `ModelMapping.cs` 双向映射 |
| 共享层 | `src/Common/CLAUDE.md` | Options 绑定模式、帮助类与扩展方法清单、常量与缓存 key 格式 |
| 入口 | `src/Entry/CLAUDE.md` | 启动流程、`Startup*.cs` partial 拆分、MVC 显式加载 controller 程序集、配置分层、管道顺序 |
| 微信 | `src/WeChat/CLAUDE.md` | 微信登录流程、`ApiGateway` 网关、未启用状态说明、启用步骤 |

## 编写原则

1. **最小化覆盖**：根 CLAUDE.md 已覆盖的内容（如命名规范、高层架构）子目录不再重复，子目录只补充**目录专属、不易从代码看出的约定**。
2. **基于实际代码**：每份文档均读取对应目录源码后编写，确保约定与实现一致（如 `ModelMapping.cs` 的 `ReverseMap`/`Ignore`、`AuditLogMiddleware` 的 Channel 批处理）。
3. **统一风格**：开头沿用项目约定 `This file provides guidance to Claude Code...`，正文用中文，代码块保持原文。
4. **面向行动**：不仅描述"是什么"，更说明"新增功能时怎么做"（如新增权限点、新增中间件、新增实体映射的具体步骤）。

## 未覆盖目录及原因

| 目录 | 规模 | 未覆盖原因 |
|------|------|----------|
| `src/Api`（根） | 少量 | 内容已分散到 `Controllers`/`Authorization`/`Middlewares` 三个子目录 |
| `src/Data`（根） | 少量 | 内容已分散到 `Entities`/`Repositories`，且 `ModelMapping` 约定已在 `Models` 中说明 |
| `src/WeChat` | 9 文件 | 已覆盖（标注未启用状态） |

## 维护建议

- 新增业务目录或约定变化时，优先在对应子目录 CLAUDE.md 补充，而非修改根文件。
- 约定失效时（如 WeChat 启用、MVC 加载方式变化）及时更新对应文档。
- 保持"一处一约定"，避免跨文件重复。
