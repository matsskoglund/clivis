variable "aws_access_key" {
  description = "The AWS access key."
}

variable "aws_secret_key" {
  description = "The AWS secret key."
}

variable "key_name" {
  description = "name of the ssh key"
  default     = "mskvb0-keypair"
}

variable "sandbox_vpc" {
  default = "vpc-957749f1"
}

variable "public_subnet_id1" {
  default = "subnet-4bbfbe3d"
}

variable "public_subnet_id2" {
  default = "subnet-496a682d"
}

variable "region" {
  default = "eu-west-1"
}

# TODO: support multiple availability zones, and default to it.
variable "availability_zone1" {
  description = "The availability zone 1"
  default     = "eu-west-1a"
}

variable "availability_zone2" {
  description = "The availability zone 2"
  default     = "eu-west-1c"
}

variable "ecs_cluster_name" {
  description = "The name of the Amazon ECS cluster."
  default     = "clivis-cluster"
}

variable "amis" {
  description = "Which AMI to spawn. Defaults to the AWS ECS optimized images."

  default = {
    eu-west-1 = "ami-a93269cf"
  }
}

variable "autoscale_min" {
  default     = "1"
  description = "Minimum autoscale (number of EC2)"
}

variable "autoscale_max" {
  default     = "4"
  description = "Maximum autoscale (number of EC2)"
}

variable "autoscale_desired" {
  default     = "2"
  description = "Desired autoscale (number of EC2)"
}

variable "instance_type" {
  default = "t2.micro"
}

variable "ssh_pubkey_file" {
  description = "Path to an SSH public key"
  default     = "/home/ubuntu/.ssh/id_rsa.pub"
}

variable "env_netatmo_password" {
  description = "The password to Netamo api"
}

variable "env_netatmo_username" {
  description = "The user id for Netamo api"
}

variable "env_netatmo_clientsecret" {
  description = "The client secret for Netamo api"
}

variable "env_netatmo_clientid" {
  description = "The client id for Netamo api"
}

variable "env_nibe_clientid" {
  description = "The client id for Nibe api"
}

variable "env_nibe_clientsecret" {
  description = "The client secret for Nibe api"
}

variable "env_nibe_redirecturl" {
  description = "The redirect url for Nibe api"
}

variable "env_nibe_host" {
  description = "The Nibe api host"
  default     = "https://api.nibeuplink.com"
}

variable "env_netatmo_host" {
  description = "The Netatmo api host"
  default     = "https://api.netatmo.com"
}

variable "image_id" {
  description = "The docker image id use"
  default     = "latest"
}
