#!/bin/bash
docker build -t matsskoglund/clivis:$BUILD_BUILDNUMBER .
docker login -u $(Docker.Username) -p $(Docker.PassWord)
docker push matsskoglund/clivis:$BUILD_BUILDNUMBER
