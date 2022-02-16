#!/bin/bash -e

dotnet publish -r linux-x64 --self-contained \
  -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/linux-x64

rm -rf bin/Publish/linux-x64/wwwroot bin/Publish/linux-x64/config

docker build --no-cache --rm -t huitian/l3a1/vector .

rm -rf dist

docker tag huitian/l3a1/vector 192.168.1.43:5000/huitian/l3a1/vector \
  && docker push 192.168.1.43:5000/huitian/l3a1/vector \
  && docker rmi 192.168.1.43:5000/huitian/l3a1/vector \
  && docker tag huitian/l3a1/vector 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d) \
  && docker push 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d) \
  && docker rmi 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d)
