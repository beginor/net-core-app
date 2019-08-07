#!/bin/bash -e
docker run --interactive --tty --rm \
  --network compose \
  --volume $(pwd)/logs:/api/logs \
  --publish 5000:80 \
  beginor/net-core-app/api
