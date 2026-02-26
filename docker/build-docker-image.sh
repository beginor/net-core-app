#!/bin/bash -e

IMAGE="beginor/net-core-app"

build_docker_image() {
    if [ -z "$1" ]; then
        echo "Please provide target platform: linux, win or osx ."
        exit 1
    fi
    if [ -z "$2" ]; then
        echo "Please provide target arch: x64 or arm64 ."
        exit 1
    fi

    local platform="$1"
    local arch="$2"
    local runtime_id="$platform-$arch"

    ./build-server.sh $runtime_id postgis
    rm -rf dist/wwwroot
    rm -rf dist/config
    rm -rf dist/cache
    rm -rf dist/storage
    rm -rf dist/log

    ./build-client.sh

    # Build docker image
    local tag="$2"
    if [ "$arch" == "x64" ]; then
        tag="amd64"
    fi
    docker build \
      --platform $platform/$tag \
      -t $IMAGE:$tag \
      --output type=image .
    rm -rf dist
}

build_docker_image "linux" "x64"
build_docker_image "linux" "arm64"

