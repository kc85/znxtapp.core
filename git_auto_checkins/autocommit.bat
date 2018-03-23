echo off

set /p str= < "C:\Users\Khanin\Google Drive\Code\znxtapp.core\gitcomment.txt"

if not defined str Echo String Length = 0 & ENDLOCAL & set strlen=0&goto:eof
echo String  %str%

call :strLen str strlen
echo String  %str% is %strlen% characters long

 
if %strlen% GTR 0 (
REM call "C:\Users\Khanin\Google Drive\Code\znxtapp.core\git_auto_checkins\gitreset.bat"
call "C:\Users\Khanin\Google Drive\Code\znxtapp.core\git_auto_checkins\copycode.bat"
call "C:\Users\Khanin\Google Drive\Code\znxtapp.core\git_auto_checkins\gitcommit.bat"
 
) else (
ECHO "string is empty"
)

exit /b

:strLen
setlocal enabledelayedexpansion

:strLen_Loop
   if not "!%1:~%len%!"=="" set /A len+=1 & goto :strLen_Loop
(endlocal & set %2=%len%)
goto :eof