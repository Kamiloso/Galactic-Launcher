@echo off
setlocal DisableDelayedExpansion
set "_DIR_=%~dp0build"
set "_REL_CERT_=release_cert.pfx"
set "_DEB_CERT_=debug_cert.pfx"

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
setlocal EnableDelayedExpansion
for %%n in (windows, linux, mac-intel, mac-silicon) do (
	if "%%n"=="windows" set "dirb=Windows" & set "cmpflag=win-x64"
	if "%%n"=="linux" set "dirb=Linux" & set "cmpflag=linux-x64"
	if "%%n"=="mac-intel" set "dirb=Mac_Intel" & set "cmpflag=osx-x64"
	if "%%n"=="mac-silicon" set "dirb=Mac_Silicon" & set "cmpflag=osx-arm64"
	
	if defined %%n (
		if exist "%_DIR_%\!dirb!\" rmdir /s /q "%_DIR_%\!dirb!"
		dotnet publish "GalacticLauncher.Frontend\GalacticLauncher.Frontend.csproj" -c Release -r !cmpflag! --self-contained -p:PublishSingleFile=true -o "%_DIR_%\!dirb!\Frontend" || goto :error
		dotnet publish "GalacticLauncher.Backend\GalacticLauncher.Backend.csproj" -c Release -r !cmpflag! --self-contained -p:PublishSingleFile=true -o "%_DIR_%\!dirb!\Backend" || goto :error
		del /f /q "%_DIR_%\!dirb!\Frontend\*.pdb" >nul 2>&1
		del /f /q "%_DIR_%\!dirb!\Backend\*.pdb" >nul 2>&1
		if not exist "%_DIR_%\!dirb!\Backend\Keys\" mkdir "%_DIR_%\!dirb!\Backend\Keys\"
		
		echo.
		
		copy ".\GalacticLauncher.Backend\Keys\%_REL_CERT_%" "%_DIR_%\!dirb!\Backend\Keys\" >nul 2>&1
		if errorlevel 1 echo Cannot copy "%_REL_CERT_%" into the !dirb! build.
		
		copy ".\GalacticLauncher.Backend\Keys\%_DEB_CERT_%" "%_DIR_%\!dirb!\Backend\Keys\" >nul 2>&1
		if errorlevel 1 echo Cannot copy "%_DEB_CERT_%" into the !dirb! build.
		
		xcopy ".\GalacticLauncher.MySql" "%_DIR_%\!dirb!\Backend\MySql" /E /I /H /Y
		if not errorlevel 1 (
			del /q /a "%_DIR_%\!dirb!\Backend\MySql\*.bat" >nul 2>&1
			if errorlevel 1 echo Cannot remove batch files from the MySql folder.
			
		) else (
			echo Cannot copy MySql Docker container instructions.
		)
		
		echo ------------------------------------------------------------
	)
)
endlocal

exit /b 0

:error
exit /b 1