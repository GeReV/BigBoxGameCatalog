pushd Catalog.Wpf

rd "bin/Release" /S /Q

dotnet publish -r win10-x64 -p:PublishSingleFile=true -c Release

popd
