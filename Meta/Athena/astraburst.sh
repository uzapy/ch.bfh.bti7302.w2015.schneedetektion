#!/bin/bash

url=http://www.astramobcam.ch/kamera
dest=/srv/athena.bfh.ch/projects/astra_burst/

# dest directories need to exist!
cams=(mvk021 mvk101 mvk105 mvk107 mvk110 mvk120 mvk122 mvk131)

date=$(date -u +%Y%m%d_%H%M%S)
echo $date

if [ ! -d ${dest} ]; then
        echo ${date} | mail -s "astrafetch - storage destination not available" bublm1@bfh.ch -- -f bublm1@bfh.ch
        exit -1 
fi

for cam in ${cams[@]}; do
        log=${dest}/${cam}/${cam}_${date}.log
        img=${dest}/${cam}/${cam}_${date}.jpg
        /usr/bin/wget ${url}/${cam}/live.jpg -O ${img} -o ${log}
done
