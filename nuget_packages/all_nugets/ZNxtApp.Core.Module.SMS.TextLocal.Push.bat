
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\src\ZNxtApp.Core.Module.SMS.TextLocal\ZNxtApp.Core.Module.SMS.TextLocal.nuspec"

nuget setApiKey  oy2dopv3ej5hu2wx53qa6ns4yuwoqo6e7ic4wvw45ixibi

nuget  push ZNxtApp.Core.Module.SMS.TextLocal.1.0.11.38791-Beta.nupkg -Source https://api.nuget.org/v3/index.json

del ZNxtApp.Core.Module.SMS.TextLocal.1.0.11.38791-Beta.nupkg