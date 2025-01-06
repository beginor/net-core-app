# ASP.NET Core MVC + Angular 的后台管理模板项目

## 已经实现的功能

- 用户登录/注销；
- 用户管理；
- 角色管理；
- 权限管理；
- 用户与角色绑定；
- 角色与权限绑定；
- 菜单管理；
- 菜单与角色绑定；
- 访问日志查看；

## 技术栈简介

### Angular 前端

前端基于 Angular 实现， 使用到的类库主要有：

- `Angular` ， 已开启 TypeScript 严格模式以及 Angular 的严格模板检查；
- `BootStrap` 以及 `Bootstrap-icons` ；
- `ng-zorro-antd`；
- `Angular Component/Material`；

Angular 多项目结构， `projects` 提供了多个三个项目，分别是：

- `web` 针对 PC 浏览器的后台管理界面， 使用 `ng-zorro-antd` 实现；
- `handset` 针对手持设备的界面， 比较简单， 仅作为示例， 使用 `Angular Component/Material` 实现；
- `app-shared` 在 `web` 和 `handset` 两个项目中共享的组件和服务；

### 后端

后端实现基于 .NET 9 实现， 使用到的类库有：

- `NHibernate` .NET 平台的老牌 ORM ， 存在多年一直都在更新维护， 非常稳定， 长期维护项目的首选；
- `NHibernate.AspNetCore.Identity` 基于 NHibernate 的 Identity 实现， 完全不依赖微软的 EntityFramework ；
- `Dapper` 灵活的 SQL 查询， 弥补 NHibernate 提供的 Linq 查询的不足；
- `AutoMapper`
- `Swashbuckle.AspNetCore` 为 API 提供基于 Swagger UI 界面；

### 数据库

默认是 PostgreSQL ， database 目录下的脚本也是基于 PostgreSQL 的； 如果需要创建其它类型的数据库， 则可以根据现有的 sql 语句进行修改；

> 为了保证开箱可用， 或许以后会切换为 SQLite 数据库；

## 部署

- 编译为 Docker 镜像进行部署；
- 编译为单个可执行文件部署；

## 功能截图

![01_login](https://beginor.github.io/assets/net-core-app/01_login.png)

![02_admin_home](https://beginor.github.io/assets/net-core-app/02_admin_home.png)

![03_admin_menu](https://beginor.github.io/assets/net-core-app/03_admin_menu.png)

![04_admin_users](https://beginor.github.io/assets/net-core-app/04_admin_users-1.png)

![05_admin_users](https://beginor.github.io/assets/net-core-app/05_admin_users-2.png)

![06_admin_roles](https://beginor.github.io/assets/net-core-app/06_admin_roles.png)

![07_admin_privileges](https://beginor.github.io/assets/net-core-app/07_admin_privileges.png)

![08_audit_log](https://beginor.github.io/assets/net-core-app/08_audit_log.png)

## 下载和使用

请参考 [下载和使用](docs/00_下载和使用.md) 。
