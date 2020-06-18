# ASP.NET Core MVC + Angular 的模板项目

注意： 本文中的命令行是指与 Linux/Unix `bash` 兼容的命令行， 如果是 Windows 系统， 可以使用 `git-bash` 或 安装 WSL 进行下面的操作。

## 重命名项目

服务端的程序集名称 `Beginor.NetCoreApp.XXX`、命名空间 (`Beginor.NetCoreApp.XXX`)、 Web 上下文 `/net-core-app` 可以通过 `rename.sh` 进行修改。

打开`rename.sh` 文件， 修改其中的 `COMPANY_NAME` 、 `PROJ_NAME` 以及 `CONTEXT_ROOT` ， 然后在命令行中执行这个脚本， 然后签入修改的内容。

## 创建数据库

依次执行 `database` 目录下的脚 SQL 本， 创建数据库， 并修改 `server/src/NetCoreApp.Api/hibernate.config` 和 `server/smart-code.yml` 中的数据库连接串。

## 运行与测试

### 前端

在命令行中打开 `client` 目录， 运行 `npm ci` 安装所需的依赖项， 然后运行 `npm run build` 进行编译。

### 服务端

1. 在命令行中打开 `server/test/NetCoreApp.Test` ， 输入 `dotnet test` ， 并查看测试结果， 应该能看到 `Test Run Successful.` 的输出；
2. 在命令行中打开 `server/src/NetCoreApp.Api` ， 输入 `dotnet run` ， 看到如下输出：

   ```sh
   Now listening on: http://localhost:5000
   Now listening on: https://localhost:5001
   Application started. Press Ctrl+C to shut down.
   ```

3. 打开浏览器， 浏览 `http://localhost:5000/net-core-app/swagger` 即可看到 swagger api 界面。

## 系统初始化

在进行访问系统之前， 需要根据 [初始化说明](docs/01_初始化.md) 进行初始化之后， 才能登录系统

## 代码生成

使用 SmartCode 代码生成器， 定制了代码生成模板， 可以根据数据表生成：

- 服务端基本的增删改查代码， 包括实体类， 仓储接口/实现类， 控制器， 单元测试；
- 客户端基本的主从形式的列表和表单， 与服务端的代码相对应；

### 准备工作

1. 克隆 `https://github.com/beginor/SmartCode.git` 至本地工作区目录（~/Projects/smartcode）；

### 生成代码操作

1. 根据业务需求创建数据表；
2. 打开命令行， 切换到 `server` 目录；
3. 输入命令 `./smartcode.sh` 即可自动生成服务端增删改查代码;
4. 运行单元测试， 确认全部单元测试通过；
5. 运行 Api 项目， 确认生成的 Api 可用；
6. 打开命令行， 切换到 `client` 目录；
7. 输入命令 `./smartcode.sh` ， 即可生成客户端的主从形式的列表和表单代码；

## 建议的开发工具

- VSCode
