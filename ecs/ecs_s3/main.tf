provider "aws" {
  region = "${var.region}"
}

resource "aws_s3_bucket" "clivis-state" {
  bucket = "mskvb0-clivis-state"
  acl    = "private"

  tags {
    Name        = "clivis-state"
    Environment = "Experiment"
  }

  versioning {
    enabled = true
  }
}
