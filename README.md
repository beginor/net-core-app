# App seed with ASP.NET Core MVC and Angular.

## 重命名项目

修改 `rename.sh` 文件中的 `COMPANY_NAME` 、 `PROJ_NAME` 以及 `CONTEXT_ROOT` ， 然后再 bash 命令行中执行。

## 数据库

依次执行 `database` 目录下的脚 SQL 本， 创建数据库， 并修改 `server/src/NetCoreApp.Api/hibernate.config` 和 `server/smart-code.yml` 中的数据库连接串。

## 运行与测试

1. 在命令行中打开 `server/test/NetCoreApp.Test` ， 输入 `dotnet test` ， 并查看测试结果， 应该能看到 `Test Run Successful.` 的输出；
2. 在命令行中打开 `server/src/NetCoreApp.Api` ， 输入 `dotnet watch run` ， 看到如下输出：

   ```
   Now listening on: http://localhost:5000
   Now listening on: https://localhost:5001
   Application started. Press Ctrl+C to shut down.
   ```

3. 打开浏览器， 浏览 `http://localhost:5000/swagger` 即可看到 swagger api 界面。

## 代码生成

### 准备工作

1. 克隆 `https://github.com/beginor/SmartCode.git` 至本地工作区目录（~/Projects/smartcode）；

### 代码生成

1. 根据业务需求创建数据表；
2. 打开命令行， 切换到 `server` 目录；
3. 在第一步的窗口中输入命令 `./smartcode.sh` 即可自动生成代码;
4. 运行单元测试， 确认全部单元测试通过；
5. 运行 Api 项目， 确认生成的 Api 可用。

## 建议使用的工具

- VSCode 
