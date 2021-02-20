#!/bin/bash -e
rm -rf dist
# Build server api image
dotnet publish -r linux-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o ./dist \
  ../server/src/NetCoreApp.Entry/NetCoreApp.Entry.csproj
# modify config file to run in stagging server;
sed -i.bak "s/ref=\"ConsoleAppender\"/ref=\"RollingFileAppender\"/g" dist/config/log.config
sed -i.bak "s/DEBUG/ERROR/g" dist/config/log.config
# sed -i.bak "s/127\.0\.0\.1/postgis/g" dist/config/hibernate.config
rm dist/config/*.bak
# Build client project;
cd ../client
rm -rf dist
npm run build-shared
npm run build-web
npm run build-handset
# Gzip static files (*.js, *.css).
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k
cd ../docker
# Copy client files to wwwroot
cp -r ../client/dist dist/wwwroot
rm -rf dist/wwwroot/app-shared
# Build docker image
