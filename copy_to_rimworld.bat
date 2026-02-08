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

xcopy "%SRC%*" "%DST%\" /E /Y /I

echo.
echo 复制完成！
pause
