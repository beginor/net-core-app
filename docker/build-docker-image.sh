#!/bin/bash -e
./build-server.sh linux-x64 postgis
rm -rf dist/config
rm -rf dist/wwwroot
rm -rf dist/log
# Build docker image
docker build --no-cache --rm -t beginor/gishub .
rm -rf dist
