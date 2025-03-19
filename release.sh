#./build.sh

rm -rf release

SYSTEM=release/userdata/system
mkdir -p $SYSTEM/services
mkdir -p $SYSTEM/trimui_sharp_led

cp src/service/bin-arm64/* $SYSTEM/trimui_sharp_led
rm $SYSTEM/trimui_sharp_led/*.dbg

cp src/knulli-service/* $SYSTEM/services

cd release
