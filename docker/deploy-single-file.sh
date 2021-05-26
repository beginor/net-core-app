#!/bin/bash -e
if [ -z "$1" ]
then
  echo "Please provide depoly type: init, update"
  exit 1
fi

if [ -z "$2" ]
then
  echo "Please provide deploy target: server:~/dotnet/"
  exit 2
fi

if [ $1 == "update" ]
then
  rm -rf dist/config
fi

scp -r dist/* $2

rm -rf dist
