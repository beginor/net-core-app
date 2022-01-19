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
dotnet publish -r $1 --self-contained --configuration Release \
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
# $2 is database server.
if [ ! -z "$2" ]
then
  echo "Change database server to $2 "
  sed -i.bak "s/127\.0\.0\.1/$2/g" dist/config/hibernate.config
fi
rm dist/config/*.bak
