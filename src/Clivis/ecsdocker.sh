#!/bin/bash
eval $(aws ecr get-login --region eu-west-1 --profile mskvb0)
docker build -t mskvb0/clivis .
docker tag mskvb0/clivis:latest 644569545355.dkr.ecr.eu-west-1.amazonaws.com/mskvb0/clivis:latest
docker push 644569545355.dkr.ecr.eu-west-1.amazonaws.com/mskvb0/clivis:latest

