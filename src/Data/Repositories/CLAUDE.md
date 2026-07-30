# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含仓储接口（`I...Repository.cs`）和实现（`...Repository.cs`），为 API 层提供数据访问能力。

## 基类与依赖

- 常规仓储继承 `HibernateRepository<Entity, Model, long>`，构造参数：`ISession session, IMapper mapper`。
- `IdentityRepository` 自行管理 Session，构造参数：`ISessionFactory factory`，实现时调用 `factory.OpenSession()`。
- 复杂查询优先使用 Dapper + SQL（如递归 CTE），简单查询使用 NHibernate.Linq。

## 开发约定

- 接口继承 `IRepository<Model, long>`，实现类继承对应 `HibernateRepository<...>`。
- 接口方法签名以 Model 为入参/出参，不直接暴露 Entity ，但特殊情况下可例外。
- 增删改操作需要：
  - 参数校验（`Argument.NotNull` / `ArgumentOutOfRange`）。
  - 使用事务（`Session.BeginTransaction()`），异常时回滚。
  - 更新审计字段（`Creator`、`Updater`、`CreatedAt`、`UpdatedAt`）。
  - 软删除通过设置 `IsDeleted = true` 实现，不物理删除。
  - 操作完成后调用 `Session.FlushAsync()` 和 `Session.Clear()`。
- 复杂查询（如树形结构、权限过滤）使用 Dapper + SQL，保持与现有风格一致。
