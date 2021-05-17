@echo off

echo Microsoft.Build.CentralPackageVersions/2.0.26
dotnet restore repro-cpv-regression.nuproj
echo Microsoft.Build.CentralPackageVersions/2.0.29
dotnet restore repro-cpv-regression2.nuproj

goto :eof