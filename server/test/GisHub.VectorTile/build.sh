#!/bin/bash -e

dotnet publish -r linux-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -p:TrimMode=link \
  -p:DebuggerSupport=false \
  -p:EnableUnsafeBinaryFormatterSerialization=false \
  -p:EnableUnsafeUTF7Encoding=false \
  -p:EventSourceSupport=false \
  -p:HttpActivityPropagationSupport=false \
  -p:InvariantGlobalization=true \
  -p:UseSystemResourceKeys=true \
  -p:TrimmerRemoveSymbols=true \
  -o ./dist

rm -rf dist/wwwroot dist/config

docker build --no-cache --rm -t huitian/l3a1/vector .

rm -rf dist

docker tag huitian/l3a1/vector 192.168.1.43:5000/huitian/l3a1/vector \
  && docker push 192.168.1.43:5000/huitian/l3a1/vector \
  && docker rmi 192.168.1.43:5000/huitian/l3a1/vector \
  && docker tag huitian/l3a1/vector 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d) \
  && docker push 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d) \
  && docker rmi 192.168.1.43:5000/huitian/l3a1/vector:$(date +%Y%m%d)
