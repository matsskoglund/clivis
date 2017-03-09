resource "aws_alb_target_group" "clivis-tg" {
  name     = "clivis-tg"
  port     = 5050
  protocol = "HTTP"
  vpc_id   = "${aws_vpc.cluster_vpc.id}"

  health_check {
    path     = "/api/climate/Ping"
    timeout  = "5"
    interval = "120"
    matcher  = "200,204"
  }
}

resource "aws_alb" "clivis-alb" {
  name            = "clivis-alb"
  security_groups = ["${aws_security_group.clivis_load_balancers.id}"]

  #subnets         = ["${var.public_subnet_id1}", "${var.public_subnet_id2}"]
  subnets = ["${aws_subnet.clivis_subnet1.id}", "${aws_subnet.clivis_subnet2.id}"]
}

resource "aws_alb_listener" "clivis-al" {
  load_balancer_arn = "${aws_alb.clivis-alb.arn}"
  port              = "5050"
  protocol          = "HTTP"

  default_action {
    target_group_arn = "${aws_alb_target_group.clivis-tg.arn}"
    type             = "forward"
  }
}

resource "aws_alb_listener_rule" "clivis-lr" {
  listener_arn = "${aws_alb_listener.clivis-al.arn}"
  priority     = 100

  action {
    type             = "forward"
    target_group_arn = "${aws_alb_target_group.clivis-tg.arn}"
  }

  condition {
    field  = "path-pattern"
    values = ["*"]
  }
}

resource "aws_ecs_task_definition" "clivis-td" {
  family                = "clivis"
  container_definitions = "${data.template_file.clivis-task-template.rendered}"

  volume {
    name      = "efs"
    host_path = "/mnt/efs/clivis"
  }
}

resource "aws_ecs_service" "clivis-service" {
  name            = "clivis-service"
  cluster         = "${aws_ecs_cluster.main.id}"
  task_definition = "${aws_ecs_task_definition.clivis-td.arn}"
  iam_role        = "${aws_iam_role.clivis_ecs_service_role.arn}"
  desired_count   = 2

  depends_on = ["aws_iam_role_policy.clivis_ecs_service_role_policy",
    "aws_alb_listener.clivis-al",
  ]

  load_balancer {
    #elb_name = "${aws_alb.clivisuix.id}"
    target_group_arn = "${aws_alb_target_group.clivis-tg.arn}"
    container_name   = "clivis"
    container_port   = 5050
  }
}
