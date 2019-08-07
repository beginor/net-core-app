#!/bin/bash -e
# push images to registry
docker tag beginor/net-core-app/nginx 192.168.6.1:5000/beginor/net-core-app/nginx \
  && docker push 192.168.6.1:5000/beginor/net-core-app/nginx \
  && docker rmi 192.168.6.1:5000/beginor/net-core-app/nginx \
  && docker tag beginor/net-core-app/nginx 192.168.6.1:5000/beginor/net-core-app/nginx:$(date +%Y%m%d) \
  && docker push 192.168.6.1:5000/beginor/net-core-app/nginx:$(date +%Y%m%d) \
  && docker rmi 192.168.6.1:5000/beginor/net-core-app/nginx:$(date +%Y%m%d)
# Deploy to server
ssh ubuntu@192.168.6.1 -t '
cd /opt/docker/net-core-app
docker-compose down
docker rmi 192.168.6.1:5000/beginor/net-core-app/nginx
docker-compose up -d
'
