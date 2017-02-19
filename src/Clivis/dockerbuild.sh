#!/bin/bash
echo "docker build -t matsskoglund/clivis:$BUILD_BUILDNUMBER ."
docker build -t matsskoglund/clivis:$BUILD_BUILDNUMBER .

echo "docker login -u $DOCKER_USERNAME -p *****"
docker login -u $DOCKER_USERNAME -p $1

echo "docker push matsskoglund/clivis:$BUILD_BUILDNUMBER"
docker push matsskoglund/clivis:$BUILD_BUILDNUMBER
