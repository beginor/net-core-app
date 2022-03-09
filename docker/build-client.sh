#!/bin/bash -e
# Build client project;
cd ../client
rm -rf dist
pnpm run build-shared
pnpm run build-web
pnpm run build-handset
cd ../docker
# Check wwwroot folder
if [ ! -d dist/wwwroot ]
then
  mkdir -p dist/wwwroot
fi
# Copy client files to wwwroot
cp -r ../client/dist/web dist/wwwroot
cp -r ../client/dist/handset dist/wwwroot
# Gzip static files (*.js, *.css).
find dist -name "*.js" -print0 | xargs -0 gzip -k
find dist -name "*.css" -print0 | xargs -0 gzip -k

# gzip client
# cd wwwroot
# tar -zcf wwwroot.tar.gz wwwroot
# mv wwwroot.tar.gz ..
# cd ..
# rm -rf dist
