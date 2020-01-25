pushd Catalog.Wpf

rd "bin/Release" /S /Q

dotnet publish -r win10-x64 -c Release

pushd bin/Release

del /Q /F /S "*.pdb"

popd

popd
