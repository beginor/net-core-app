#!/bin/bash -e

# 公司名称
COMPANY_NAME=MyCompany
# 服务端项目名称
PROJ_NAME=TplApp
# 部署虚拟目录
CONTEXT_ROOT=tpl-app
# 新的服务端项目前缀（公司名称+项目名称, 特殊符号需要用 \ 进行转义）
SERVER_PREFIX="${COMPANY_NAME}.${PROJ_NAME}"
# 修改 Docker 编译/部署文件
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/build-docker-image.sh
sed -i.bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/build-docker-image.sh
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/deploy-docker-image.sh
sed -i.bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/deploy-docker-image.sh
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/Dockerfile
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./docker/Dockerfile
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/docker-compose.yml
sed -i.bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/docker-compose.yml
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下 Docker 文件
git add *
git commit -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 修改服务端相关文件
sed -i.bak "s/net-core-app/${CONTEXT_ROOT}/g" ./src/Entry/Properties/launchSettings.json
grep Beginor.NetCoreApp -rl src --include *.cs | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include *.hbm.xml | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include *.config | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include *.config | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl src --include *.csproj | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl test --include *.cs | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl test --include *.csproj | xargs sed -i.bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"

sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/Entry.csproj
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/Startup.Swagger.cs
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Data/Repositories/AppStorageRepository.cs
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/config/hibernate.config
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./src/Entry/config/log.config

sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ./smartcode.yml
sed -i.bak "s/Beginor/${COMPANY_NAME}/g" ./smartcode.yml
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git add -f ./.gitlab-ci.yml
git add -f ./.vscode/launch.json
git add -f ./.vscode/tasks.json
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 移动文件至新的目录
git mv NetCoreApp.sln ${PROJ_NAME}.sln
# 提交一下服务端文件
git add *
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
# 修改项目引用路径
sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g" ${PROJ_NAME}.sln
grep NetCoreApp -rl server --include *.csproj | xargs sed -i.bak "s/NetCoreApp/${PROJ_NAME}/g"
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git commit --amend -m "Rename to ${COMPANY_NAME}.${PROJ_NAME}"
