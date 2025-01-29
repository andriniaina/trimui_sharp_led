#./build.sh

rm -rf release

mkdir -p release/services
mkdir -p release/trimui_sharp_led

cp src/service/bin-arm64/* release/trimui_sharp_led
rm release/trimui_sharp_led/*.dbg

cp src/knulli-service/* release/services

cd release
zip -r trimui_sharp_led-release.zip services trimui_sharp_led
