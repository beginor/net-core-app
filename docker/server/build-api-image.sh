#!/bin/bash -e
# Build and publish server api project.
dotnet publish -c Release -o dist ../../server/src/NetCoreApp.Api/NetCoreApp.Api.csproj
# modify config file to run in stagging server;
sed -i.bak "s/ref=\"ConsoleAppender\"/ref=\"RollingFileAppender\"/g" dist/log.config
sed -i.bak "s/DEBUG/ERROR/g" dist/log.config
sed -i.bak "s/127\.0\.0\.1/postgis/g" dist/hibernate.config
rm dist/*.bak
# Build docker image
docker build --no-cache --rm -t beginor/net-core-app/api .
rm -rf dist
