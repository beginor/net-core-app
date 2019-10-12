#!/bin/bash -e
# Build and publish server api project.
cd ../../server/src/NetCoreApp.Api/
rm -rf bin
dotnet publish -c Release
# Move publish output to docker folder
mv bin/Release/netcoreapp3.0/publish/ ../../../docker/server/dist
rm -rf bin
cd ../../../docker/server
# modify config file to run in stagging server;
sed -i.bak "s/ref=\"ConsoleAppender\"/ref=\"RollingFileAppender\"/g" dist/log.config
sed -i.bak "s/127\.0\.0\.1/postgis/g" dist/hibernate.config
rm dist/*.bak
# Build docker image
docker build --no-cache --rm -t beginor/net-core-app/api .
rm -rf dist
