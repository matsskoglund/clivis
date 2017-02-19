#!/bin/bash
docker build -t matsskoglund/clivis:$BUILD_BUILDNUMBER .
echo "Username = $Docker_UserName - $DOCKER_USERNAME"
docker login -u $Docker_UserName -p $Docker_PassWord
docker push matsskoglund/clivis:$BUILD_BUILDNUMBER
