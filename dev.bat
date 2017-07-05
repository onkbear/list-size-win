@echo off

set ARG=%1
if "%1" == "" (
    set ARG=build
)

if "%ARG%" == "build" (
    call :build
    if ERRORLEVEL 1 goto :error
)

if "%ARG%" == "debug" (
    call :debug-build
    if ERRORLEVEL 1 goto :error
    call :test
    if ERRORLEVEL 1 goto :error
)

if "%ARG%" == "test" (
    call :test
    if ERRORLEVEL 1 goto :error
)

exit /b 0


:debug-build
csc.exe /debug+ /out:.\Bin\lss.exe Src\*.cs
if ERRORLEVEL 1 exit /b 1
exit /b 0

:build
csc.exe /optimize /out:.\Bin\lss.exe Src\*.cs
if ERRORLEVEL 1 exit /b 1
exit /b 0

:test
.\bin\lss.exe
if ERRORLEVEL 1 exit /b 1
exit /b 0

:error
echo Error !!
exit /b 1
