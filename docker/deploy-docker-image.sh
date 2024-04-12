#!/bin/bash -e

REGISTRY="127.0.0.1:5000"
IMAGE="beginor/net-core-app"
TAG=$(date +"%Y%m%d%H%M%S")

# push images to registry
docker tag $IMAGE $REGISTRY/$IMAGE \
  && docker push $REGISTRY/$IMAGE \
  && docker rmi $REGISTRY/$IMAGE \
  && docker tag $IMAGE $REGISTRY/$IMAGE:$TAG \
  && docker push $REGISTRY/$IMAGE:$TAG \
  && docker rmi $REGISTRY/$IMAGE:$TAG
# Deploy to server
ssh ubuntu@127.0.0.1 -t '
cd /opt/docker/net-core-app
docker compose pull
docker compose up -d
'
