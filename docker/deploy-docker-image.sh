#!/bin/bash -e
# push images to registry
docker tag beginor/net-core-app 192.168.6.1:5000/beginor/net-core-app \
  && docker push 192.168.6.1:5000/beginor/net-core-app \
  && docker rmi 192.168.6.1:5000/beginor/net-core-app \
  && docker tag beginor/net-core-app 192.168.6.1:5000/beginor/net-core-app:$(date +%Y%m%d) \
  && docker push 192.168.6.1:5000/beginor/net-core-app:$(date +%Y%m%d) \
  && docker rmi 192.168.6.1:5000/beginor/net-core-app:$(date +%Y%m%d)
# Deploy to server
ssh ubuntu@192.168.6.1 -t '
cd /opt/docker/net-core-app
docker-compose down
docker rmi 192.168.6.1:5000/beginor/net-core-app
docker-compose up -d
'
