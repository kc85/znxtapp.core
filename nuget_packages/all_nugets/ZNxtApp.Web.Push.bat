
cd C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\all_nugets

nuget pack "C:\Users\Khanin\Google Drive\Code\znxtapp.core\nuget_packages\ZNxtApp.Web\Beta\ZNxtApp.Web.nuspec"

nuget setApiKey  oy2n5qf7jzhihb4tihpqlf2rr6mv3sfe7dxbt5uvginwja

nuget  push ZNxtApp.Web.1.0.10-Beta.nupkg -Source https://api.nuget.org/v3/index.json
 
del ZNxtApp.Web.1.0.10-Beta.nupkg