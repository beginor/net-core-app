#!/bin/bash -e
./build-to-single-file.sh linux-x64 postgis
# Build docker image
docker build --no-cache --rm -t beginor/net-core-app .
rm -rf dist
