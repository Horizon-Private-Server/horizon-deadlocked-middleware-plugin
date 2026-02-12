#!/bin/bash
set -e

sudo rm -rf out
sudo rm -rf middleware
sudo rm -rf out/plugin
mkdir -p out/plugin
mkdir -p middleware

cp ../horizon-server-database-middleware middleware -r

docker build . -t deadlocked_middleware_plugin

docker run -v "${PWD}/out/":/mnt/out -it deadlocked_middleware_plugin

sudo chmod a+rw out -R

