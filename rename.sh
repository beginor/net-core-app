#!/bin/bash -e

# 公司名称
COMPANY_NAME=MyCompany
# 服务端项目名称
PROJ_NAME=TplApp
# 部署虚拟目录
CONTEXT_ROOT=tpl-app
# 新的服务端项目前缀（公司名称+项目名称, 特殊符号需要用 \ 进行转义）
SERVER_PREFIX="${COMPANY_NAME}.${PROJ_NAME}"
# Docker 用户名（公司名小写）
DOCKER_USER="$(echo "${COMPANY_NAME}" | tr '[:upper:]' '[:lower:]')"
# 修改 Docker 编译/部署文件（注意实际文件名是 docker-compose.yaml，不是 .yml）
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/build-docker-image.sh
sed -i.bak "s/beginor/${DOCKER_USER}/g" ./docker/build-docker-image.sh
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/deploy-docker-image.sh
sed -i.bak "s/beginor/${DOCKER_USER}/g" ./docker/deploy-docker-image.sh
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/Dockerfile
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./docker/Dockerfile
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/docker-compose.yaml
sed -i.bak "s/beginor/${DOCKER_USER}/g" ./docker/docker-compose.yaml
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下 Docker 文件
git add *
git commit -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 修改服务端相关文件
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./src/Entry/Properties/launchSettings.json
# 缓存 key 前缀（Beginor.NetCoreApp 命名空间替换由下方 grep 循环统一处理）
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./src/Common/CacheKeyFormat.cs
# 调试配置指向 Entry 程序集（AssemblyName 会被改成 ${PROJ_NAME}），需同步更新
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./.vscode/launch.json
grep Beginor.NetCoreApp -rl src --include '*.cs'      | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include '*.hbm.xml' | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include '*.config'  | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include '*.csproj'  | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl test --include '*.cs'     | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl test --include '*.csproj' | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
# 修改各目录 CLAUDE.md：仅替换项目命名空间 Beginor.NetCoreApp，
# 保留 Beginor.AppFx.* / Beginor.AspNetCore.* 等外部 NuGet 包引用
grep -rl "Beginor.NetCoreApp" --include CLAUDE.md . | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"

sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/Entry.csproj
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Data/Repositories/AppStorageRepository.cs
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/config/hibernate.config
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/config/log.config

sed -i.bak "s/Beginor/${COMPANY_NAME}/g" ./smartcode.yml
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./smartcode.yml

sed -i.bak "s/Beginor/${COMPANY_NAME}/g" ./CLAUDE.md
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./CLAUDE.md
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./CLAUDE.md

# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git add -f ./.gitlab-ci.yml
git add -f ./.vscode/launch.json
git add -f ./.vscode/tasks.json
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 移动文件至新的解决方案文件名
git mv NetCoreApp.slnx ${PROJ_NAME}.slnx
# 提交一下服务端文件
git add *
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 删除备份文件
find . -name '*.bak' -delete
git add *
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
