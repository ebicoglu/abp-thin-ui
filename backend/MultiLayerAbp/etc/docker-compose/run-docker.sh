#!/bin/bash

if [[ ! -d certs ]]
then
    mkdir certs
    cd certs/
    if [[ ! -f localhost.pfx ]]
    then
        dotnet dev-certs https -v -ep localhost.pfx -p a00051f3-9bb9-49cf-bfea-1a77138db958 -t
    fi
    cd ../
fi

docker-compose up -d
