#!/bin/bash -e
# Build client project;
cd ../../net-core-app-client
rm -rf dist
pnpm build
cd ../net-core-app/docker
# Check wwwroot folder
if [ -d dist/wwwroot ]
then
  rm -rf mkdir -p dist/wwwroot
fi
mkdir -p dist/wwwroot
# Copy client files to wwwroot
cp -r ../../net-core-app-client/dist dist/wwwroot/web
# Gzip static files (*.js, *.css).
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k
