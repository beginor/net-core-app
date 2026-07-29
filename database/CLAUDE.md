# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

# 数据库规范

- 数据库名、表名、字段名均使用小写字母加下划线分割（`snake_case`）
- 表名使用英文复数形式，表示集合（如 `users`、`order_items`）
- 主键统一命名为 `id` ，数据类型为 `bigint` ，默认值为 `snow_flake_id()`
- 外键格式为 `表名单数形式_字段名_id`（如 `user_id`、`order_item_id`）

例外情况：

- `000_aspnet_core_identity.sql` 中的数据表是 ASP.NET Core Identity 框架的固有限制（主键泛型默认 string）
- `006_audit_logs.sql` 由于考虑日志很快、存在高并发的情况，所以直接采用 `bigint` 作为其主键

# 数据库表设计原则

设计数据库表时尽量满足关系数据库的第一范式（1NF）、第二范式（2NF）、第三范式（3NF）。

# sql 文件规范

1. sql 文件以 3 位数字 + 数据表名称命名， 100 以下框架公共表， 100 以上是业务表；
2. 保证 sql 文件按照开头的数字从小到大依次执行可以创建全新的的数据库；

# 软删除

需要软删除时，使用 `999_add_common_cols.sql` 添加软删除需要的表字段；
