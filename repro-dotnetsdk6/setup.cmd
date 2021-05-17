@echo off
if NOT EXIST .\packages\dotnetsdk31 call :dotnet-install -Version 3.1.409 -InstallDir .\packages\dotnetsdk31 || exit /b 1
if NOT EXIST .\packages\dotnetsdk60 call :dotnet-install -Version 6.0.100-preview.3.21202.5 -InstallDir .\packages\dotnetsdk60 || exit /b 3
echo set DOTNET_MULTILEVEL_LOOKUP=0
set DOTNET_MULTILEVEL_LOOKUP=0
echo done
goto :eof



:dotnet-install
echo running dotnet-install.ps1 %*
set _args=%*
set _args=%_args:"="""%
powershell.exe -ExecutionPolicy Bypass -NonInteractive -NoProfile -Nologo -Command "& (iex """{$(irm https://dot.net/v1/dotnet-install.ps1)}""") %_args%"
set _args=
goto :eof