#!/bin/bash -e
# Build server api image
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

docker build --no-cache --rm -t huitian/l3a1/vector .

rm -rf dist
