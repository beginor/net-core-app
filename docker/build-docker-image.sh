#!/bin/bash -e
./build-single-file.sh linux-x64 lighthouse
rm -rf dist/wwwroot
rm -rf dist/config
# Build docker image
docker build --no-cache --rm -t beginor/gishub .
rm -rf dist
