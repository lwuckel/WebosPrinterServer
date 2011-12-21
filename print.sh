#!/bin/bash
#
# Parameter: pcl-file color-mode plex-mode
#
# To convert pcl to ps you have to use ghostpcl
# http://ghostscript.com/GhostPCL.html
#
pcl6 -sDEVICE=pswrite -dNOPAUSE -dBATCH -r600 -sOutputFile=$1.ps $1
lpr $1.ps
