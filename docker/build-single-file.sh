#!/bin/bash -e
# $1 is target runtime id;
if [ -z "$1" ]
then
  echo "Please provide target runtime id like: linux-x64, win-x64 or osx-x64 ."
  exit 1
fi
echo "Target runtime id is $1"
rm -rf dist
# Build server api image
dotnet publish -r $1 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=false \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o ./dist \
  ../server/src/GisHub.Entry/GisHub.Entry.csproj
# modify config file to run in stagging server;
sed -i.bak "s/ref=\"ConsoleAppender\"/ref=\"RollingFileAppender\"/g" dist/config/log.config
sed -i.bak "s/DEBUG/ERROR/g" dist/config/log.config
# $2 is database server.
if [ ! -z "$2" ]
then
  echo "Change database server to $2 "
  sed -i.bak "s/127\.0\.0\.1/$2/g" dist/config/hibernate.config
fi
rm dist/config/*.bak
# Check wwwroot folder
if [ ! -d dist/wwwroot ]
then
  mkdir -p dist/wwwroot
fi
# Build client project;
cd ../client
rm -rf dist
pnpm run build-shared
pnpm run build-web
pnpm run build-handset
# Gzip static files (*.js, *.css).
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k
cd ../docker
# Copy client files to wwwroot
cp -r ../client/dist/web dist/wwwroot
cp -r ../client/dist/handset dist/wwwroot
