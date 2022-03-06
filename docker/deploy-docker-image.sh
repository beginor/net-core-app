#!/bin/bash -e
# push images to registry
docker tag beginor/gishub 81.71.93.249:5000/beginor/gishub \
  && docker push 81.71.93.249:5000/beginor/gishub \
  && docker rmi 81.71.93.249:5000/beginor/gishub \
  && docker tag beginor/gishub 81.71.93.249:5000/beginor/gishub:$(date +%Y%m%d) \
  && docker push 81.71.93.249:5000/beginor/gishub:$(date +%Y%m%d) \
  && docker rmi 81.71.93.249:5000/beginor/gishub:$(date +%Y%m%d)
# Deploy to server
ssh lighthouse -t '
cd docker/gishub
docker-compose down
docker rmi 127.0.0.1:5000/beginor/gishub
docker-compose up -d
'
