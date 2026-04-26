@echo off
chcp 65001 >nul

set SRC=%~dp0
set DST=E:\steam\steamapps\common\RimWorld\Mods\StrinovaMod

echo Copy from:
echo %SRC%
echo To:
echo %DST%
echo.

if not exist "%DST%" (
    echo Creating target folder...
    mkdir "%DST%"
)

for %%F in (About Assemblies Defs Textures) do (
    echo Copying %%F ...
    xcopy "%SRC%%%F" "%DST%\%%F\" /E /Y /I /Q
)

echo.
echo Done!
pause