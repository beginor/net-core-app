#!/bin/bash -e
# push images to registry
docker tag beginor/gishub 192.168.6.1:5000/beginor/gishub \
  && docker push 192.168.6.1:5000/beginor/gishub \
  && docker rmi 192.168.6.1:5000/beginor/gishub \
  && docker tag beginor/gishub 192.168.6.1:5000/beginor/gishub:$(date +%Y%m%d) \
  && docker push 192.168.6.1:5000/beginor/gishub:$(date +%Y%m%d) \
  && docker rmi 192.168.6.1:5000/beginor/gishub:$(date +%Y%m%d)
# Deploy to server
ssh ubuntu@192.168.6.1 -t '
cd /opt/docker/gishub
docker-compose down
docker rmi 192.168.6.1:5000/beginor/gishub
docker-compose up -d
'
