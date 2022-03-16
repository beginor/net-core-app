#!/bin/bash -e
./build-server.sh linux-x64 postgis
rm -rf dist/config
rm -rf dist/wwwroot
# rm -rf dist/log
./build-client.sh

# Build docker image
docker buildx build --pull --rm \
  --platform linux/amd64 \
  -t 192.168.1.13:5000/beginor/net-core-app \
  --push .

rm -rf dist
