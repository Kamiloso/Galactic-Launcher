@echo off
setlocal DisableDelayedExpansion
set "_DIR_=%~dp0build"

dotnet clean

if "%~1"=="" (
    set "windows=1"
    set "linux=1"
    set "mac-intel=1"
    set "mac-silicon=1"
    goto :build
)

:check
set "_a=%~1"
if defined _a (
    for %%n in (windows, linux, mac-intel, mac-silicon) do (
        if "%_a%"=="%%n" set "%%n=1"
    )
    shift
    goto :check
)

:build
:: WINDOWS
if defined windows (
    if exist "%_DIR_%\Windows\" rmdir /s /q "%_DIR_%\Windows"
    dotnet publish "GalacticLauncher.Frontend\GalacticLauncher.Frontend.csproj" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Windows\Frontend" || goto :error
    dotnet publish "GalacticLauncher.Backend\GalacticLauncher.Backend.csproj" -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Windows\Backend" || goto :error
    del /f /q "%_DIR_%\Windows\Frontend\*.pdb" >nul 2>&1
    del /f /q "%_DIR_%\Windows\Backend\*.pdb" >nul 2>&1
	if not exist "%_DIR_%\Windows\Backend\Keys\" mkdir "%_DIR_%\Windows\Backend\Keys\"
	copy ".\GalacticLauncher.Backend\Keys\release_cert.pfx" "%_DIR_%\Windows\Backend\Keys\" >nul 2>&1
	copy ".\GalacticLauncher.Backend\Keys\debug_cert.pfx" "%_DIR_%\Windows\Backend\Keys\" >nul 2>&1
)

:: LINUX
if defined linux (
    if exist "%_DIR_%\Linux\" rmdir /s /q "%_DIR_%\Linux"
    dotnet publish "GalacticLauncher.Frontend\GalacticLauncher.Frontend.csproj" -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Linux\Frontend" || goto :error
    dotnet publish "GalacticLauncher.Backend\GalacticLauncher.Backend.csproj" -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Linux\Backend" || goto :error
    del /f /q "%_DIR_%\Linux\Frontend\*.pdb" >nul 2>&1
    del /f /q "%_DIR_%\Linux\Backend\*.pdb" >nul 2>&1
	if not exist "%_DIR_%\Linux\Backend\Keys\" mkdir "%_DIR_%\Linux\Backend\Keys\"
	copy ".\GalacticLauncher.Backend\Keys\release_cert.pfx" "%_DIR_%\Linux\Backend\Keys\" >nul 2>&1
	copy ".\GalacticLauncher.Backend\Keys\debug_cert.pfx" "%_DIR_%\Linux\Backend\Keys\" >nul 2>&1
)

:: MAC INTEL
if defined mac-intel (
    if exist "%_DIR_%\Mac_Intel\" rmdir /s /q "%_DIR_%\Mac_Intel"
    dotnet publish "GalacticLauncher.Frontend\GalacticLauncher.Frontend.csproj" -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Mac_Intel\Frontend" || goto :error
    dotnet publish "GalacticLauncher.Backend\GalacticLauncher.Backend.csproj" -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Mac_Intel\Backend" || goto :error
    del /f /q "%_DIR_%\Mac_Intel\Frontend\*.pdb" >nul 2>&1
    del /f /q "%_DIR_%\Mac_Intel\Backend\*.pdb" >nul 2>&1
	if not exist "%_DIR_%\Mac_Intel\Backend\Keys\" mkdir "%_DIR_%\Mac_Intel\Backend\Keys\"
	copy ".\GalacticLauncher.Backend\Keys\release_cert.pfx" "%_DIR_%\Mac_Intel\Backend\Keys\" >nul 2>&1
	copy ".\GalacticLauncher.Backend\Keys\debug_cert.pfx" "%_DIR_%\Mac_Intel\Backend\Keys\" >nul 2>&1
)

:: MAC SILICON
if defined mac-silicon (
    if exist "%_DIR_%\Mac_Silicon\" rmdir /s /q "%_DIR_%\Mac_Silicon"
    dotnet publish "GalacticLauncher.Frontend\GalacticLauncher.Frontend.csproj" -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Mac_Silicon\Frontend" || goto :error
    dotnet publish "GalacticLauncher.Backend\GalacticLauncher.Backend.csproj" -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -o "%_DIR_%\Mac_Silicon\Backend" || goto :error
    del /f /q "%_DIR_%\Mac_Silicon\Frontend\*.pdb" >nul 2>&1
    del /f /q "%_DIR_%\Mac_Silicon\Backend\*.pdb" >nul 2>&1
	if not exist "%_DIR_%\Mac_Silicon\Backend\Keys\" mkdir "%_DIR_%\Mac_Silicon\Backend\Keys\"
	copy ".\GalacticLauncher.Backend\Keys\release_cert.pfx" "%_DIR_%\Mac_Silicon\Backend\Keys\" >nul 2>&1
	copy ".\GalacticLauncher.Backend\Keys\debug_cert.pfx" "%_DIR_%\Mac_Silicon\Backend\Keys\" >nul 2>&1
)

exit /b 0

:error
exit /b 1