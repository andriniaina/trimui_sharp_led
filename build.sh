# build
#docker build --pull -t cross-build-arm64 -f Dockerfile.cross-build-x64-arm64 .
docker run --rm -v $(pwd)/src/service:/source -w /source cross-build-arm64 dotnet publish -a arm64 -c Release -o bin-arm64 -p:SysRoot=/crossrootfs/arm64 -p:LinkerFlavor=lld
