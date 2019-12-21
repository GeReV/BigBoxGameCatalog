pushd Catalog.Wpf

dotnet publish -r win10-x64 -p:PublishSingleFile=true -c Release

popd
