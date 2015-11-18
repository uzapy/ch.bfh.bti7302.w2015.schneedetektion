#!/bin/bash
date=$(date +%Y%m%d_%H%M%S)
used=$(du /srv/athena.bfh.ch/projects/astra -ch)
echo "${used} from ${date}" | mail -s "astrafetch - reminder" bublm1@bfh.ch -- -f bublm1@bfh.ch
