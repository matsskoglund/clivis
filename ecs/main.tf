provider "aws" {
    region = "${var.region}"
}

#resource "aws_key_pair" "auth" {
#   key_name   = "${var.key_name}"
 #  public_key = "${file(var.ssh_pubkey_file)}"
#}

resource "aws_security_group" "clivis-load_balancers" {
    name = "clivis-load_balancers"
    description = "Allows all traffic"
    
    vpc_id = "${var.sandbox_vpc}"


    ingress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }

    # TODO: this probably only needs egress to the ECS security group.
    egress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }
}

resource "aws_security_group" "clivis-sg" {
    name = "clivis-sg"
    description = "Allows all traffic"
    
    vpc_id = "${var.sandbox_vpc}"

    # TODO: remove this and replace with a bastion host for SSHing into
    # individual machines.
    ingress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }

    ingress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        security_groups = ["${aws_security_group.clivis-load_balancers.id}"]
    }

    egress {
        from_port = 0
        to_port = 0
        protocol = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }
}


resource "aws_ecs_cluster" "main" {
    name = "${var.ecs_cluster_name}"
}

resource "aws_autoscaling_group" "clivis-ag" {
    availability_zones = ["${var.availability_zone1}", "${var.availability_zone2}"]
    name = "${var.ecs_cluster_name}"
    min_size = "${var.autoscale_min}"
    max_size = "${var.autoscale_max}"
    desired_capacity = "${var.autoscale_desired}"
    health_check_type = "EC2"
    launch_configuration = "${aws_launch_configuration.clivis-lc.name}"
    #vpc_zone_identifier = ["${aws_subnet.main.id}"]
    vpc_zone_identifier = ["${var.public_subnet_id1}", "${var.public_subnet_id2}"]
}

resource "aws_launch_configuration" "clivis-lc" {
    name = "clivis-${var.ecs_cluster_name}"
    image_id = "${lookup(var.amis, var.region)}"
    instance_type = "${var.instance_type}"
    security_groups = ["${aws_security_group.clivis-sg.id}"]
    iam_instance_profile = "${aws_iam_instance_profile.clivis-ip.name}"
   # key_name = "${aws_key_pair.auth.key_name}"
    associate_public_ip_address = true
    user_data = "#!/bin/bash\necho ECS_CLUSTER='${var.ecs_cluster_name}' > /etc/ecs/ecs.config"
}

resource "aws_iam_role" "ecs_host_role" {
    name = "ecs_host_role"
    assume_role_policy = "${file("policies/ecs-role.json")}"
}

resource "aws_iam_role_policy" "ecs_instance_role_policy" {
    name = "ecs_instance_role_policy"
    policy = "${file("policies/ecs-instance-role-policy.json")}"
    role = "${aws_iam_role.ecs_host_role.id}"
}

resource "aws_iam_role" "ecs_service_role" {
    name = "ecs_service_role"
    assume_role_policy = "${file("policies/ecs-role.json")}"
}

resource "aws_iam_role_policy" "ecs_service_role_policy" {
    name = "ecs_service_role_policy"
    policy = "${file("policies/ecs-service-role-policy.json")}"
    role = "${aws_iam_role.ecs_service_role.id}"
}

resource "aws_iam_instance_profile" "clivis-ip" {
    name = "clivis-instance-profile"
    path = "/"
    roles = ["${aws_iam_role.ecs_host_role.name}"]
}

resource "aws_iam_role_policy" "ecr_container_policy" {
    name = "ecr_container_policy"
    policy = "${file("policies/ecr-role-policy.json")}"
    role = "${aws_iam_role.ecs_host_role.id}"
}

resource "aws_iam_role_policy" "log_policy" {
    name = "log_policy"
    policy = "${file("policies/log-policy.json")}"
    role = "${aws_iam_role.ecs_host_role.id}"
}


resource "aws_cloudwatch_log_group" "clivis-lg" {
  name = "clivis-lg"
 retention_in_days = "7"
  tags {
    Environment = "experimentation"
    Application = "clivis"
  }
}

resource "aws_cloudwatch_log_stream" "clivis-log-stream" {
  name           = "clivis-log-stream"
  log_group_name = "${aws_cloudwatch_log_group.clivis-lg.name}"
}

resource "template_file" "clivis-task-template" {
  template = "${file("task-definitions/clivis-task-template.json.tpl")}"

  vars {
    env_netatmo_password = "${var.env_netatmo_password}"
    env_netatmo_username = "${var.env_netatmo_username}"
    env_netatmo_clientsecret = "${var.env_netatmo_clientsecret}"
    env_netatmo_clientid = "${var.env_netatmo_clientid}"
    env_nibe_clientid = "${var.env_nibe_clientid}"
    env_nibe_clientsecret = "${var.env_nibe_clientsecret}"
    env_nibe_redirecturl = "${var.env_nibe_redirecturl}" 
    image_id = "${var.image_id}"
  }
}