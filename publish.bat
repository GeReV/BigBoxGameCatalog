@REM Use Publish Catalog.Wpf configuration from IDE.
@REM Figure out why published package didn't run.

pushd Catalog.Wpf

rd "bin/Release" /S /Q

dotnet publish -r win-x64 -c Release

pushd "bin/Release"

del /Q /F /S "*.pdb"
del /Q /F /S "*.md"

popd

popd
