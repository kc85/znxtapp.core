﻿

set http_base_url=http://localhost/ZNxtApp.Core.WebTest
set mudule_root_path=c:\temp\ZNxtApp

set version=%1
set nuget_path=%2
set bin_path=%3
set nuget_spec_path=%4
set project_name=%5
set project_path=%6
set nuget_version=%version%-Beta
set nuget_path="%nuget_path%"

set command="(gc \"%nuget_spec_path%\") -replace 'version>([^\s]+)<\/version', 'version>%nuget_version%</version' | Out-File \"%nuget_spec_path%\" -Encoding UTF8"

powershell -Command %command%

cd %nuget_path%

nuget pack %nuget_spec_path%

move %project_name%.%nuget_version%.nupkg %mudule_root_path%\%project_name%_%nuget_version%.zip

icacls %mudule_root_path%\%project_name%_%nuget_version%.zip /grant Users:F

cd %project_path%

call .\build-events-scripts\post-build\winhttpjs.bat "%http_base_url%/api/module/reinstall?module_name=%project_name%/%nuget_version%"  -method POST   -reportfile reportfile_app.txt
