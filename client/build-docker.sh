#!/bin/bash
set -e

npm run build
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k

docker build --no-cache --rm -t beginor/net-core-app .
