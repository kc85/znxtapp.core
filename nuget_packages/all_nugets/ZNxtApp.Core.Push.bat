
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\ZNxtApp.Core\Alpha\ZNxtApp.Core.nuspec"

nuget setApiKey  oy2gpagwhqqokhdxiyiquhclnthm4cguli74yuabzegnia

nuget  push ZNxtApp.Core.1.0.8-Beta.nupkg -Source https://api.nuget.org/v3/index.json

del ZNxtApp.Core.1.0.8-Beta.nupkg