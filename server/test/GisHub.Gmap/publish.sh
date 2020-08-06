#!/bin/bash -e
rm -rf ./bin/Publish
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=None -r linux-x64 -o ./bin/Publish/linux-x64
#
# cp -r config ./bin/Publish/linux-x64
# scp -r bin/Publish/linux-x64/* yuezhengtu@172.21.68.126:~/mng/data/gmap/
