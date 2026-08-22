# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录是 NUnit 测试项目，覆盖 API 控制器、仓储、安全（Identity）、Common 帮助类与配置。测试复用应用真实的 DI 容器，多数测试连接真实数据库（PostgreSQL）。

## 运行

```bash
cd test/Test && dotnet test                                   # 运行全部测试
cd test/Test && dotnet test --filter FullyQualifiedName~UsersControllerTest
```

> 运行前需本地 PostgreSQL 可用，并已执行 `database/` 下的脚本初始化 schema；首次还需先跑 `SystemTest` 初始化权限、角色、管理员用户与菜单。

## 测试基类

- `BaseTest`（抽象）：构造函数中 `InitTest()` 组装 DI 容器——用 `TestHostEnvironment`（环境名 `Development`，ContentRoot 指向输出目录）加载 `config/` 下的 `appsettings*.json`/`log.config`，调用 `new Startup(config, env).ConfigureServices(services)`，因此容器与生产应用一致。
- `BaseTest<T>`：`Target => ServiceProvider.GetService(typeof(T)) as T`，测试类继承它即可注入被测对象（如 `BaseTest<UsersController>`、`BaseTest<UserManager<AppUserEntity>>`）。
- `CreateTestPrincipal()`：从数据库取 `admin` 用户构造 `ClaimsPrincipal`（`TestAuth` scheme），供需要登录态的测试用。

## 目录结构

- `Api/`：控制器测试（`UsersControllerTest`、`RolesControllerTest`、`App*ControllerTest`）。
- `Data/`：仓储测试（`App*RepositoryTest`，继承 `BaseTest<I...Repository>`）、`HibernateTest`（`ISessionFactory`）、`ModelMappingTest`（AutoMapper 双向映射）。
- `Security/`：`UserManagerTest`、`RoleManagerTest`。
- `Common/`：`EntityHelper`、`FileHelper`、`FileStorage`、`SafeUrlEncoder`、`Captcha` 等帮助类测试。
- `ConfigTest` / `SystemTest`：根命名空间下，测配置选项解析与系统初始化。

## 约定

- 测试类用 `[TestFixture]`，命名空间 `Beginor.NetCoreApp.Test.*`；`Test.csproj` 关闭 nullable（`<Nullable>disable</Nullable>`）。
- 测试方法以 `_01_`、`_02_` 数字前缀命名，体现执行/阅读顺序；断言用 NUnit `Assert.That(...)`。
- 配置文件在 `Test.csproj` 中从 `src/Entry/config/` 链接并复制到输出目录（`hibernate.config`、`log.config`、`appsettings*.json`），因此运行时 `config/` 相对路径可用。
- `SystemTest` 是系统初始化入口：`_01_CanInitSystem` 创建权限（`IsRequired = true` 的必需权限）、管理员/匿名用户角色、`admin` 用户与菜单导航项；`_02_CanResetPassword` 测密码重置。新增业务模块后，需在 `CreatePrivilegesAsync` 的 `AppPrivilegeModel[]` 补充对应权限，否则动态授权策略解析不到该权限。
- 新增测试时，按被测对象所属层放入对应子目录，继承 `BaseTest<TargetType>`，用 `Target` 访问实例。
