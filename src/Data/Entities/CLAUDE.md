# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含所有 NHibernate 实体类，使用 `NHibernate.Mapping.Attributes` 进行 ORM 映射。

## 开发约定

添加新的实体类时，务必遵守下面的约定：

- 所有实体使用 `#nullable disable`，保持与现有代码一致。
- 主键统一继承 `BaseEntity<long>` ，使用 `long` 类型，生成策略为 `trigger-identity`。
- 关联字段使用 `ManyToOne` + `NotFound = NotFoundMode.Ignore`，避免关联数据缺失导致查询失败。
- 需要软删除的表在 `[Class]` 上设置 `Where = "is_deleted = false"`。
- 数组类型字段（如 `Roles`、`Privileges`）映射为 PostgreSQL 数组，声明为 `Type = "string[]"`。
