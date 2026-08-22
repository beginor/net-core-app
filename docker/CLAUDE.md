# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this directory.

## 概览

本目录包含服务器与 Docker 镜像的构建、部署脚本及 `docker-compose.yaml`。构建产物统一输出到 `dist/`（已在 `.gitignore`）。

## 脚本清单

- `build-server.sh <runtime-id> [db-server]`：`dotnet publish` 发布 `src/Entry`，单文件、自包含、Release；发布后：
  - `sed` 把 `log.config` 的 `ConsoleAppender` 改为 `RollingFileAppender`、日志级别 `DEBUG`→`ERROR`；
  - 若传入 `$2`，把 `hibernate.config` 的 `127.0.0.1` 替换为数据库服务器地址；
  - 清理产生的 `*.bak`。
  - `<runtime-id>` 如 `linux-x64`、`win-x64`、`osx-x64`。
- `deploy-server.sh <mode> <target>`：`<mode>` 为 `init` 或 `update`（`update` 会先删 `dist/config` 再覆盖）；`scp -r dist/* <target>` 上传，最后清理 `dist`。`<target>` 形如 `server:~/dotnet/`。
- `build-docker-image.sh` / `deploy-docker-image.sh`：Docker 镜像的构建与推送，镜像标签使用时间戳（见最近提交）。
- `build-client.sh`：构建前端资源。
- `Dockerfile`：服务器镜像定义。

## 部署流程

1. `./build-server.sh linux-x64 [db-server]` 生成 `dist/`；
2. `./deploy-server.sh update server:~/dotnet/` 上传产物；
3. 或用 `build-docker-image.sh` + `deploy-docker-image.sh` 走镜像部署。

## 约定

- 构建脚本通过 `sed -i.bak` 在发布后改写配置文件，避免改动源码里的配置；改动构建逻辑时沿用此模式，并记得 `rm *.bak`。
- 新增部署配置项时，在对应脚本里用 `sed` 替换，保持「构建期改写、运行时只读」的约定。
