#!/bin/bash -e

rm -rf dist

dotnet publish -r linux-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o ./dist/linux-x64

dotnet publish -r win-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o ./dist/win-x64
rm -rf ./dist/win-x64/dist

dotnet publish -r osx-x64 -c Release \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:SelfContained=true \
  -p:PublishReadyToRun=false \
  -p:DebugType=None \
  -o ./dist/osx-x64
rm -rf ./dist/osx-x64/dist
