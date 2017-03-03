#!/bin/bash
#terraform apply -var "aws_access_key=$AWS_ACCESS_KEY" -var "aws_secret_key=$AWS_SECRET_KEY" -var "env_netatmo_password=$ENV_NETATMO_PASSWORD" -var "env_netatmo_username=$ENV_NETATMO_USERNAME" -var "env_netatmo_clientsecret=$ENV_NETATMO_CLIENTSECRET" -var "env_netatmo_clientid=$ENV_NETATMO_CLIENTID" -var "env_nibe_clientid=$ENV_NIBE_CLIENTID" -var "env_nibe_clientsecret=$ENV_NIBE_CLIENTSECRET" -var "env_nibe_redirecturl=$ENV_NIBE_REDIRECTURL"  -var "image_id=$BUILD_BUILDID"
terraform apply -var "aws_access_key=$1" -var "aws_secret_key=$2" -var "env_netatmo_password=$3" -var "env_netatmo_username=$4" -var "env_netatmo_clientsecret=$5" -var "env_netatmo_clientid=$6" -var "env_nibe_clientid=$7" -var "env_nibe_clientsecret=$9" -var "env_nibe_redirecturl=$9"  -var "image_id=$10"


