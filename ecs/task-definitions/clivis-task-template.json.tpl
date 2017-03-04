[
    {
        "name": "clivis",
        "image": "644569545355.dkr.ecr.eu-west-1.amazonaws.com/mskvb0/clivis:${image_id}",
        "cpu": 10,
        "memory": 300,
        "links": [],
        "portMappings": [
            {
                "containerPort": 5050,
                "hostPort": 0,
                "protocol": "tcp"
            }
        ],
        "essential": true,
        "entryPoint": [],
        "command": [],        
         "mountPoints": [
        {
          "containerPath": "/app/data",
          "sourceVolume": "efs",
          "readOnly": null
        }
      ],
        "volumesFrom": [],
      "environment": [
        {
          "name": "NETATMO_CLIENTSECRET",
          "value": "${env_netatmo_clientsecret}"
        },
        {
          "name": "NETATMO_PASSWORD",
          "value": "${env_netatmo_password}"
        },
        {
          "name": "NETATMO_CLIENTID",
          "value": "${env_netatmo_clientid}"
        },
        {
          "name": "NETATMO_USERNAME",
          "value": "${env_netatmo_username}"
        },
        {
          "name": "NIBE_CLIENTID",
          "value": "${env_nibe_clientid}"
        },
        {
          "name": "NIBE_REDIRECTURL",
          "value": "${env_nibe_redirecturl}"
        },
        {
          "name": "NIBE_HOST",
          "value": "${env_nibe_host}"
        },
        {
          "name": "NETATMO_HOST",
          "value": "${env_netatmo_host}"
        },
        {
          "name": "BUILD_VERSION",
          "value": "${image_id}"
        },
        {
          "name": "NIBE_CLIENTSECRET",
          "value": "${env_nibe_clientsecret}"
        }
      ],
         "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "clivis-lg",
          "awslogs-region": "eu-west-1",
          "awslogs-stream-prefix": "clivis-log-stream"
        }
      }
    }
]