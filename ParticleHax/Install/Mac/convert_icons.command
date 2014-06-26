#!/bin/sh

cd "`dirname "$0"`"
iconutil -c icns icons.iconset
mv icons.icns ParticleHax.app/Contents/Resources/