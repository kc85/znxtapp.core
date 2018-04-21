
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\src\ZNxtApp.Core.Module.App\ZNxtApp.Core.Module.App.nuspec"

nuget setApiKey  oy2defvbvddr2coisssau26mnxcvdiuu5peynhwfsim5dy

nuget  push ZNxtApp.Core.Module.App.1.0.8.40473-Beta.nupkg -Source https://api.nuget.org/v3/index.json

del ZNxtApp.Core.Module.App.1.0.8.40473-Beta.nupkg