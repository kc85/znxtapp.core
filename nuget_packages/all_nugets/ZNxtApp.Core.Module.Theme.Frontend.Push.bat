
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\src\ZNxtApp.Core.Module.Theme.Frontend\ZNxtApp.Core.Module.Theme.Frontend.nuspec"

nuget setApiKey  oy2p6v7zo5rwupm2zccqh5l5dvpynbcelnloygxgfnmqtq

nuget  push ZNxtApp.Core.Module.Theme.Frontend.1.0.11.38791-Beta.nupkg -Source https://api.nuget.org/v3/index.json

del ZNxtApp.Core.Module.Theme.Frontend.1.0.11.38791-Beta.nupkg