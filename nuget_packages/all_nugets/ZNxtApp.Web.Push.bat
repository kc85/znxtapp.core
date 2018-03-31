
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\ZNxtApp.Web\Alpha\ZNxtApp.Web.nuspec"

rem nuget setApiKey  oy2gpagwhqqokhdxiyiquhclnthm4cguli74yuabzegnia

rem nuget  push ZNxtApp.Core.1.0.4-Alpha.nupkg -Source https://api.nuget.org/v3/index.json