
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\src\ZNxtApp.Core.Module\ZNxtApp.Core.Module.nuspec"

nuget setApiKey  oy2e7wviower6phb6qzqvkjxk6l67abezdu3kbhlwbad6q

rem nuget  push ZNxtApp.Core.Module.1.0.10-Alpha.nupkg -Source https://api.nuget.org/v3/index.json