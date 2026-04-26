@echo off
@chcp 65001 >nul
set "SRC=%~dp0"
set "DST=E:\steam\steamapps\common\RimWorld\Mods\StrinovaMod"

echo 正在从：
echo %SRC%
echo 复制到：
echo %DST%
echo.

if not exist "%DST%" (
    echo 目标目录不存在，正在创建...
    mkdir "%DST%"
)

for %%F in (About Assemblies Defs Textures) do (
    echo 复制 %%F ...
    xcopy "%SRC%%%F" "%DST%\%%F\" /E /Y /I /Q
)

echo.
echo 复制完成！
pause
