@echo off
chcp 65001 > nul
echo Запускаю юнити...

set "MY_PATH=%~dp0"
set "MY_PATH=%MY_PATH:~0,-1%"

start "" "C:\Program Files\Unity\Hub\Editor\6000.4.8f1\Editor\Unity.exe" -projectPath "%MY_PATH%"