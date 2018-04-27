# ZNxtApp

ZNxtApp is next generation application development framework for web application in .NET. It is lightweight, high performance and customizable framework suitable for all your modern day web development requirement.

- Developed in .Net 4.5.2
- High performance and scalable
- Plug and play
- Highly customizable

Let's join us on the next generation web development journey, it's free and open source

# Quick Start:
### Prerequisites
  - .Net 4.5.2 
  - MongoDB 3.x
### Installation
  - Create new Empty WebApplication project from your Visual Studio. (Make sure project .net  version 4.5.2)
  - Install ZNxtApp.Web Nuget package (Install-Package ZNxtApp.Web) 
  - Update below setting on your new web application 
   ```sh <appSettings>
    <!-- Put you application name here -->
    <add key="AppName" value="ZNxtAppDummyApp" />
    <!-- this is the unique key of your application. Should be unique in your environments  -->
    <add key="AppId" value="b758bd68-e295-4088-a340-5114761c66e4" />
    <!-- This is required in case if you are running application under some virtual directory in IIS-->
    <add key="AppPath" value="/ZNxtApp.Core.WebTest" />
    <!-- ZNxtApp required write access to this path -->
    <add key="ModuleCachePath" value="C:\temp\ZNxtApp" />
    <!-- MongoDb name should be unique to that server-->
    <add key="DataBaseName" value="ZNxtAppTest" />
    <!-- mongoDB connection string-->
    <add key="MongoDBConnectionString" value="mongodb://localhost:27017" />
    <!-- AppMode values  :  Maintenance, Debug, Live -->
    <add key="AppMode" value="Maintenance" /></appSettings>
```
 ```sh
 <system.webServer>
     <httpProtocol>
      <customHeaders>
        <clear />
        <remove name="X-Powered-By" />
        <remove name="X-AspNet-Version"/>
        <remove name="X-AspNetMvc-Version"/>
        <remove name="X-AspNet-Version"/>                
      </customHeaders>
    </httpProtocol>
    <handlers>
      <add name="RequestHandler" verb="*" path="*" type="ZNxtApp.Core.Web.Handler.RequestHandler"  allowPathInfo ="true"  preCondition ="integratedMode" />
    </handlers>
  </system.webServer>
  ``` 
 ```sh 
  Global.asax
----
using ZNxtApp.Core.Web.AppStart;
-----
    public class Global : GlobalAsaxBase
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
        }
    }
 ```
 - Run your Web Application you should see the ZNxtApp installation page
 - Installation will take few minute. After you are ready to use ZNxtApp first application
 - Start your journey. Next create your own module ->>  
  
### Nuget Packages 
- [ZNxtApp.Core](https://www.nuget.org/packages/ZNxtApp.Core)
- [ZNxtApp.Core.Module](https://www.nuget.org/packages/ZNxtApp.Core.Module)
- [ZNxtApp.Core.Module.App](https://www.nuget.org/packages/ZNxtApp.Core.Module.App)
- [ZNxtApp.Core.Module.Theme](https://www.nuget.org/packages/ZNxtApp.Core.Module.Theme)
- [ZNxtApp.Web](https://www.nuget.org/packages/ZNxtApp.Web)
- [ZNxtApp.Core.Module.SMS.TextLocal](https://www.nuget.org/packages/ZNxtApp.Core.Module.SMS.TextLocal)
 
### License

----

GNU


**Free Software!**
