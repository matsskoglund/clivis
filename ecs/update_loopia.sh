#!/bin/bash
python3 ../loopiaupdate.py -u $1 -p $2 --ip $(terraform output alb-dns-name) $3