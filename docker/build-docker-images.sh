#!/bin/bash -e
# Build client image;
cd client
./build-nginx-image.sh
cd ..
# Build server api image
cd server
./build-api-image.sh
cd ..
