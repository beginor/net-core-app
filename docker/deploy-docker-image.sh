#!/bin/bash -e
# push images to registry
docker tag beginor/net-core-app 127.0.0.1:5000/beginor/net-core-app \
  && docker push 127.0.0.1:5000/beginor/net-core-app \
  && docker rmi 127.0.0.1:5000/beginor/net-core-app \
  && docker tag beginor/net-core-app 127.0.0.1:5000/beginor/net-core-app:$(date +%Y%m%d) \
  && docker push 127.0.0.1:5000/beginor/net-core-app:$(date +%Y%m%d) \
  && docker rmi 127.0.0.1:5000/beginor/net-core-app:$(date +%Y%m%d)
# Deploy to server
ssh ubuntu@127.0.0.1 -t '
cd /opt/docker/net-core-app
docker compose up -d --pull=always
'
