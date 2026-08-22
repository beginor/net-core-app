# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含请求/响应模型、搜索条件与 DTO。模型只承载数据，不含业务逻辑；实体（`src/Data/Entities`）与模型之间的映射集中在 `src/Data/ModelMapping.cs`（AutoMapper Profile）。

## 基类（来自 Beginor.AppFx.Core）

- `StringEntity`：主键为 `string` 的实体基类（如 `AppUserModel`、`AppUserTokenModel`）。
- `PaginatedRequestModel`：分页请求基类，含 `Skip`/`Take` 等；搜索模型继承它（如 `UserSearchRequestModel`、`AppUserTokenSearchModel`）。
- `StringIdNameEntity`：`Id` + `Name` 的轻量模型，用于下拉/选择等场景。

## 开发约定

- 所有模型文件以 `#nullable disable` 开头，保持与现有代码一致。
- 类与属性用中文 XML 文档注释（`/// <summary>`）说明用途。
- 校验用 `System.ComponentModel.DataAnnotations` 特性（`[Required]`、`[Compare]` 等），`ErrorMessage` 用中文；校验由模型绑定/MVC 自动触发。
- 命名约定：
  - 请求/响应模型 `XxxModel`；
  - 搜索参数 `XxxSearchModel`，继承 `PaginatedRequestModel`；
  - 模型类名与对应实体名保持一致（如 `AppUserEntity` ↔ `AppUserModel`）。

## 与实体的映射

- 实体 ↔ 模型的互转在 `src/Data/ModelMapping.cs` 的 `ModelMapping : AutoMapper.Profile` 中维护，用 `CreateMap<A, B>().ReverseMap()`。
- 需要自定义映射时用 `.ForMember(dest => ..., map => map.MapFrom(src => ...))`；反向映射忽略只读/派生字段用 `.Ignore()`（如 `AppUserToken` 的 `User`、`AppAttachment` 的 `Creator`/`CreatedAt`）。
- 新增实体与模型时，记得在 `ModelMapping` 补充 `CreateMap`，否则 AutoMapper 运行时会报错。
- 派生属性（如 `AppUserModel.DisplayName = Surname + GivenName`）在映射里用 `MapFrom` 指定来源。
