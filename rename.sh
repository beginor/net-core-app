#!/bin/bash -e

# 客户端应用名称
NEW_CLIENT_NAME=soil-env-gis
# 公司名称
COMPANY_NAME=HuiTian
# 服务端项目名称
PROJ_NAME=SoilEnvGIS
# 新的服务端项目前缀（公司名称+项目名称, 特殊符号需要用 \ 进行转义）
SERVER_PREFIX="${COMPANY_NAME}.${PROJ_NAME}"

# 修改客户端相关文件
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/build-docker.sh
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/default.conf
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/Dockerfile
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/e2e/src/app.e2e-spec.ts
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/e2e/src/home/home.e2e-spec.ts
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/package-lock.json
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./client/package.json
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./docker-compose.test.yml
sed -i .bak "s/net-core-app/${NEW_CLIENT_NAME}/g" ./docker-compose.yml
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下客户端文件
git add *
git commit -m "Rename client app"
# 修改服务端相关文件
grep Beginor.NetCoreApp  -rl server --include *.cs | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp  -rl server --include *.csproj | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
grep Beginor.NetCoreApp  -rl server --include *.config | xargs sed -i .bak "s/Beginor\.NetCoreApp/${SERVER_PREFIX}/g"
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
grep NetCoreApp  -rl server --include *.csproj | xargs sed -i .bak "s/NetCoreApp/${PROJ_NAME}/g"
# 删除备份文件
find . -name '*.bak' -delete
# 提交一下服务端文件
git add *
git commit -m "Update server references."
