@echo off
setlocal enabledelayedexpansion

set size=0
for /f "tokens=*" %%x in ('dir /s /a /b %1') do set /a size+=%%~zx
echo.!size!

endlocal
