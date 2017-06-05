#!/bin/bash
docker stop $(docker ps -q 2>/dev/null) 2>/dev/null
docker rm -v $(docker ps --filter status=exited -q 2>/dev/null) 2>/dev/null
docker rmi -f $(docker images -q 2>/dev/null) 2>/dev/null
