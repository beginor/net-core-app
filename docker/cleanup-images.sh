#!/bin/bash -e
# remove images without a tag
docker rmi $(docker images -f "dangling=true" -q)
