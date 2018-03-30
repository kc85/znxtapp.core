
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\src\ZNxtApp.Core.Module.Theme\ZNxtApp.Core.Module.Theme.nuspec"

nuget setApiKey  oy2jjgmvgdoclr3alu2qnaq6oqris32upeqogl56tadogi

nuget  push ZNxtApp.Core.Module.Theme.1.0.6-Alpha.nupkg -Source https://api.nuget.org/v3/index.json