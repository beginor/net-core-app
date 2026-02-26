#!/bin/bash -e

REGISTRY="127.0.0.1:5000"

IMAGE="beginor/net-core-app"
IMAGE_AMD64="$IMAGE:amd64"
IMAGE_ARM64="$IMAGE:arm64"

TAG=$(date +"%Y%m%d%H%M%S")

# push images to registry
docker tag $IMAGE_AMD64 $REGISTRY/$IMAGE_AMD64
docker tag $IMAGE_AMD64 $REGISTRY/$IMAGE_AMD64-$TAG
docker push $REGISTRY/$IMAGE_AMD64-$TAG

docker tag $IMAGE_ARM64 $REGISTRY/$IMAGE_ARM64
docker tag $IMAGE_ARM64 $REGISTRY/$IMAGE_ARM64-$TAG
docker push $REGISTRY/$IMAGE_ARM64-$TAG

# create and push manifest
docker manifest create --insecure $REGISTRY/$IMAGE:$TAG \
  $REGISTRY/$IMAGE_ARM64-$TAG \
  $REGISTRY/$IMAGE_AMD64-$TAG
docker manifest push --insecure $REGISTRY/$IMAGE:$TAG

# cleanup
docker manifest rm $REGISTRY/$IMAGE:$TAG
docker rmi $IMAGE_ARM64 $IMAGE_AMD64 \
  $REGISTRY/$IMAGE_ARM64 $REGISTRY/$IMAGE_AMD64 \
  $REGISTRY/$IMAGE_ARM64-$TAG $REGISTRY/$IMAGE_AMD64-$TAG

echo "Final image is: $REGISTRY/$IMAGE:$TAG"

# Deploy to server
ssh ubuntu@127.0.0.1 -t '
cd /opt/docker/net-core-app
docker compose pull
docker compose up -d
'
