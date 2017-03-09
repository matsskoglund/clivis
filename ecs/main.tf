provider "aws" {
  region = "${var.region}"
}

#resource "aws_key_pair" "auth" {
#  key_name   = "${var.key_name}"
#  public_key = "${file(var.ssh_pubkey_file)}"
#}

resource "aws_security_group" "clivis_load_balancers" {
  name        = "clivis_load_balancers"
  description = "Allows all traffic"

  vpc_id = "${aws_vpc.cluster_vpc.id}"

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # TODO: this probably only needs egress to the ECS security group.
  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_security_group" "clivis-sg" {
  name        = "clivis-sg"
  description = "Allows all traffic"

  vpc_id = "${aws_vpc.cluster_vpc.id}"

  # TODO: remove this and replace with a bastion host for SSHing into
  # individual machines.
  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port       = 0
    to_port         = 0
    protocol        = "-1"
    security_groups = ["${aws_security_group.clivis_load_balancers.id}"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_ecs_cluster" "main" {
  name = "${var.ecs_cluster_name}"
}

resource "aws_autoscaling_group" "clivis-ag" {
  availability_zones   = ["${var.availability_zone1}", "${var.availability_zone2}"]
  name                 = "${var.ecs_cluster_name}"
  min_size             = "${var.autoscale_min}"
  max_size             = "${var.autoscale_max}"
  desired_capacity     = "${var.autoscale_desired}"
  health_check_type    = "EC2"
  launch_configuration = "${aws_launch_configuration.clivis-lc.name}"

  #key_name = "${aws_key_pair.auth.key_name}"
  #vpc_zone_identifier = ["${aws_subnet.main.id}"]
  # vpc_zone_identifier = ["${var.public_subnet_id1}", "${var.public_subnet_id2}"]
  vpc_zone_identifier = ["${aws_subnet.clivis_subnet1.id}", "${aws_subnet.clivis_subnet2.id}"]
}

resource "aws_launch_configuration" "clivis-lc" {
  name                 = "${var.ecs_cluster_name}"
  image_id             = "${lookup(var.amis, var.region)}"
  instance_type        = "${var.instance_type}"
  security_groups      = ["${aws_security_group.clivis-sg.id}"]
  iam_instance_profile = "${aws_iam_instance_profile.clivis-ip.name}"

  key_name                    = "${var.key_name}"
  associate_public_ip_address = true
  user_data                   = "#!/bin/bash\necho ECS_CLUSTER='${var.ecs_cluster_name}' > /etc/ecs/ecs.config"
}

resource "aws_iam_role" "clivis_ecs_host_role" {
  name               = "clivis_ecs_host_role"
  assume_role_policy = "${file("policies/ecs-role.json")}"
}

resource "aws_iam_role_policy" "clivis_ecs_instance_role_policy" {
  name   = "clivis_ecs_instance_role_policy"
  policy = "${file("policies/ecs-instance-role-policy.json")}"
  role   = "${aws_iam_role.clivis_ecs_host_role.id}"
}

resource "aws_iam_role" "clivis_ecs_service_role" {
  name               = "clivis_ecs_service_role"
  assume_role_policy = "${file("policies/ecs-role.json")}"
}

resource "aws_iam_role_policy" "clivis_ecs_service_role_policy" {
  name   = "clivis_ecs_service_role_policy"
  policy = "${file("policies/ecs-service-role-policy.json")}"
  role   = "${aws_iam_role.clivis_ecs_service_role.id}"
}

resource "aws_iam_instance_profile" "clivis-ip" {
  name  = "clivis_instance_profile"
  path  = "/"
  roles = ["${aws_iam_role.clivis_ecs_host_role.name}"]
}

resource "aws_iam_role_policy" "clivis_ecr_container_policy" {
  name   = "clivis_ecr_container_policy"
  policy = "${file("policies/ecr-role-policy.json")}"
  role   = "${aws_iam_role.clivis_ecs_host_role.id}"
}

resource "aws_iam_role_policy" "clivis_log_policy" {
  name   = "clivis_log_policy"
  policy = "${file("policies/log-policy.json")}"
  role   = "${aws_iam_role.clivis_ecs_host_role.id}"
}

resource "aws_cloudwatch_log_group" "clivis-lg" {
  name              = "clivis-lg"
  retention_in_days = "7"

  tags {
    Environment = "experimentation"
    Application = "clivis"
  }
}

#resource "aws_cloudwatch_log_stream" "clivis-log-stream" {
#  name           = "clivis-log-stream"
#  log_group_name = "${aws_cloudwatch_log_group.clivis-lg.name}"
#}

data "template_file" "clivis-task-template" {
  template = "${file("task-definitions/clivis-task-template.json.tpl")}"

  vars {
    env_netatmo_password     = "${var.env_netatmo_password}"
    env_netatmo_username     = "${var.env_netatmo_username}"
    env_netatmo_clientsecret = "${var.env_netatmo_clientsecret}"
    env_netatmo_clientid     = "${var.env_netatmo_clientid}"
    env_nibe_clientid        = "${var.env_nibe_clientid}"
    env_nibe_clientsecret    = "${var.env_nibe_clientsecret}"
    env_nibe_redirecturl     = "${var.env_nibe_redirecturl}"
    env_nibe_host            = "${var.env_nibe_host}"
    env_netatmo_host         = "${var.env_netatmo_host}"
    image_id                 = "${var.image_id}"
    log-group                = "${aws_cloudwatch_log_group.clivis-lg.name}"
    log-stream               = "${aws_cloudwatch_log_stream.clivis-log-stream.name}"
  }
}

###################### VPC and subnets and network stuff

resource "aws_vpc" "cluster_vpc" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true

  tags {
    Name = "clivis-vpc"
  }
}

resource "aws_subnet" "clivis_subnet1" {
  vpc_id                  = "${aws_vpc.cluster_vpc.id}"
  cidr_block              = "10.0.0.0/24"
  availability_zone       = "eu-west-1c"
  map_public_ip_on_launch = true

  tags {
    Name = "clivis_subnet1"
  }
}

resource "aws_subnet" "clivis_subnet2" {
  vpc_id                  = "${aws_vpc.cluster_vpc.id}"
  cidr_block              = "10.0.1.0/24"
  availability_zone       = "eu-west-1a"
  map_public_ip_on_launch = true

  tags {
    Name = "clivis_subnet2"
  }
}

resource "aws_internet_gateway" "clivis-gw" {
  vpc_id = "${aws_vpc.cluster_vpc.id}"

  tags {
    Name = "clivis-gw"
  }
}

resource "aws_route_table" "clivis-eu-west-public" {
  vpc_id = "${aws_vpc.cluster_vpc.id}"

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = "${aws_internet_gateway.clivis-gw.id}"
  }

  tags {
    Name = "Public Subnet"
  }
}

resource "aws_route_table_association" "clivis-eu-west-1c-public" {
  subnet_id      = "${aws_subnet.clivis_subnet1.id}"
  route_table_id = "${aws_route_table.clivis-eu-west-public.id}"
}

# Using the same routing table for both subnet at the moment
#resource "aws_route_table" "clivis-eu-west-1a-public" {
#  vpc_id = "${var.cluster_vpc}"

#  route {
#    cidr_block = "0.0.0.0/0"
#    gateway_id = "${aws_internet_gateway.gw.id}"
#  }

#  tags {
#    Name = "Public Subnet"
#  }
#}

resource "aws_route_table_association" "clivis-eu-west-1a-public" {
  subnet_id      = "${aws_subnet.clivis_subnet2.id}"
  route_table_id = "${aws_route_table.clivis-eu-west-public.id}"
}

output "ip" {
  value = "${aws_alb.clivis-alb.dns_name}"
}
