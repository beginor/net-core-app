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
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/client/build-nginx-image.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/client/build-nginx-image.sh
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/client/default.conf
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/client/deploy-nginx-image.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/client/deploy-nginx-image.sh
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/client/Dockerfile
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/client/Dockerfile

sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/server/build-api-image.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/server/build-api-image.sh
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./docker/server/build-api-image.sh
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/server/deploy-api-image.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/server/deploy-api-image.sh
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/server/Dockerfile
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/server/Dockerfile
sed -i .bak "s/Beginor/${COMPANY_NAME}/g" ./docker/server/Dockerfile
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./docker/server/Dockerfile

sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/build-docker-images.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/build-docker-images.sh
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/docker-compose.yml
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/docker-compose.yml
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./docker/deploy-images.sh
sed -i .bak "s/beginor/$(echo ${COMPANY_NAME} | tr '[:upper:]' '[:lower:]')/g" ./docker/deploy-images.sh
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下 Docker 文件
git add *
git commit -m "Rename docker files"
# 修改客户端相关文件
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/web/e2e/src/app.e2e-spec.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/web/e2e/src/home/home.e2e-spec.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/handset/e2e/src/app.e2e-spec.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/handset/e2e/src/home/home.e2e-spec.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/package-lock.json
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/package.json
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/web/src/environments/environment.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/web/src/environments/environment.prod.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/handset/src/environments/environment.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/projects/handset/src/environments/environment.prod.ts
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./client/proxy.conf.json
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下客户端文件
git add *
git commit -m "Rename client app"
# 修改服务端相关文件
sed -i .bak "s/net-core-app/${CONTEXT_ROOT}/g" ./server/src/NetCoreApp.Api/Properties/launchSettings.json
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./server/src/NetCoreApp.Api/Properties/launchSettings.json
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./server/src/NetCoreApp.Api/Startup.Swagger.cs
grep Beginor.NetCoreApp -rl server --include *.cs | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl server --include *.hbm.xml | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl server --include *.config | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl server --include *.csproj | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp -rl server --include *.config | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./server/test/NetCoreApp.Test/NetCoreApp.Test.csproj
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./.gitlab-ci.yml
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./.vscode/launch.json
sed -i .bak "s/Beginor/${COMPANY_NAME}/g" ./.vscode/launch.json
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./.vscode/tasks.json
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" ./server/smartcode.yml
sed -i .bak "s/Beginor/${COMPANY_NAME}/g" ./server/smartcode.yml
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git commit -m "Rename server app"
# 移动文件至新的目录
git mv server/src/NetCoreApp.Api/NetCoreApp.Api.csproj server/src/NetCoreApp.Api/${PROJ_NAME}.Api.csproj
git mv server/src/NetCoreApp.Api server/src/${PROJ_NAME}.Api
git mv server/src/NetCoreApp.Data/NetCoreApp.Data.csproj server/src/NetCoreApp.Data/${PROJ_NAME}.Data.csproj
git mv server/src/NetCoreApp.Data server/src/${PROJ_NAME}.Data
git mv server/src/NetCoreApp.Models/NetCoreApp.Models.csproj server/src/NetCoreApp.Models/${PROJ_NAME}.Models.csproj
git mv server/src/NetCoreApp.Models server/src/${PROJ_NAME}.Models
git mv server/src/NetCoreApp.Services/NetCoreApp.Services.csproj server/src/NetCoreApp.Services/${PROJ_NAME}.Services.csproj
git mv server/src/NetCoreApp.Services server/src/${PROJ_NAME}.Services
git mv server/test/NetCoreApp.Test/NetCoreApp.Test.csproj server/test/NetCoreApp.Test/${PROJ_NAME}.Test.csproj
git mv server/test/NetCoreApp.Test server/test/${PROJ_NAME}.Test
git mv server/NetCoreApp.sln server/${PROJ_NAME}.sln
# 提交一下服务端文件
git add *
git commit -m "Move server app"
# 修改项目引用路径
sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g" server/${PROJ_NAME}.sln
grep NetCoreApp -rl server --include *.csproj | xargs sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g"
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git commit -m "Update server references."
