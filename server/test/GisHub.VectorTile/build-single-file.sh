#!/bin/bash -e

rm -rf bin/Publish

dotnet publish -r linux-x64 --self-contained \
  -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/linux-x64
cp run.sh bin/Publish/linux-x64

dotnet publish -r win-x64 --self-contained \
  -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/win-x64
cp run.bat bin/Publish/win-x64

dotnet publish -r osx-x64 --self-contained \
  -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o bin/Publish/osx-x64
cp run.sh bin/Publish/osx-x64
