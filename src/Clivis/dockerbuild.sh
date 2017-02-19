#!/bin/bash
docker build -t matsskoglund/clivis:$BUILD_BUILDNUMBER .
docker login -u $Docker_UserName -p $Docker_PassWord
docker push matsskoglund/clivis:$BUILD_BUILDNUMBER
