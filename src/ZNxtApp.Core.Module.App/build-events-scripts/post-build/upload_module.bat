

rem set http_base_url=http://localhost/ZNxtApp.Core.WebTest
set http_base_url=https://ZNxt.App

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

cd %project_path%

echo building nuget
 .\build-events-scripts\post-build\nuget pack %nuget_spec_path%

 echo moving  nuget
 move %project_name%.%nuget_version%.nupkg %project_name%_%nuget_version%.zip

echo uploading nuget the application 
 .\build-events-scripts\post-build\ZNxtApp.CLI -u  %http_base_url%/upload/upload_module.z  "%project_name%_%nuget_version%.zip

 echo delete  nuget 
 del %project_name%_%nuget_version%.zip
 
 echo Deploy module 
 call .\build-events-scripts\post-build\winhttpjs.bat "%http_base_url%/api/module/reinstall?module_name=%project_name%/%nuget_version%"  -method POST   -reportfile reportfile_app.txt
