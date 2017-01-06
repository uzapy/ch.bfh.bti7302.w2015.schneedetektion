#!/bin/bash

# Online-Webcams der ASTRA
url=http://www.astramobcam.ch/kamera
# File-Share auf athena
dest=/srv/athena.bfh.ch/projects/astra/

# Liste der Kameras erstellen. Ziel-Ordner müssen existieren!
cams=(mvk021 mvk101 mvk105 mvk107 mvk110 mvk120 mvk122 mvk131)

# Aktuelles Datum auslese in der Form JahrMonatTag_StundeMinuteSekunde
date=$(date -u +%Y%m%d_%H%M%S)
echo $date

# Überprüfen ob auf den File-Share zugeriffen werden kann
# Wenn nicht, wird abgebrochen und eine Meldung per Mail versendet
if [ ! -d ${dest} ]; then
	echo ${date} | mail -s "astrafetch - storage destination not available" vep2@bfh.ch -- -f vep2@bfh.ch
	exit -1	
fi

# Pro Kamera in der Liste
for cam in ${cams[@]}; do
	# Leere Log-Datei erstellen
	log=${dest}/${cam}/${cam}_${date}.log
	#Leere JPG-Datei erstellen
	img=${dest}/${cam}/${cam}_${date}.jpg
	# Kamerabild per WGET herunterladen.
	# Daten in JPG-Datei schreiben und Log in Log-Datei schrieben
	/usr/bin/wget ${url}/${cam}/live.jpg -O ${img} -o ${log}
done
