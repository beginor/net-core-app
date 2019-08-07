#!/bin/bash -e
# Build client, if you want to change what to build, please modify the `build`
# script in package.json
npm run build
# Gzip static files (*.js, *.css).
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k
# Build client image (nginx).
docker build --no-cache --rm -t beginor/net-core-app/nginx .
