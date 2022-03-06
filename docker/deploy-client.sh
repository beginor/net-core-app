#!/bin/bash -e

scp -r wwwroot.tar.gz lighthouse:~/docker/gishub/

ssh lighthouse -t '
cd docker/gishub
rm -rf wwwroot/*
tar -zxvf wwwroot.tar.gz
rm wwwroot.tar.gz
'

rm wwwroot.tar.gz
