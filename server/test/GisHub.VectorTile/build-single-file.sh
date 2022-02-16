#!/bin/bash -e

rm -rf dist

dotnet publish -r linux-x64 --self-contained \
  -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/linux-x64

dotnet publish -r win-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/win-x64

dotnet publish -r osx-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/osx-x64
