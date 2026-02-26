@echo off
chcp 65001 > nul
echo Сиська шишка заливаем
set /p userinput=Название комита скотины(меня):
git add .
git commit -m "%userinput%"
git push origin main
pause