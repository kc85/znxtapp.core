# ZNxtApp

<<<<<<< HEAD
ZNxtApp is next generation application development framework for web application. ZNxtApp is lightweight high performance customizable suitable for all your modern web development requirement. Its provide plug and play injectable module without impacting application ongoing sessions. Its usages Nosql database for high performance and easy deployment for modern day dynamic, arbitrary data structure.

Let's join us on the next generation web development journey it's free and open source.
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
  
 
 License
----

=======
ZNxtApp is an next generation application development framework for web devlopment. It is lightweight, scalable and high performance framework suitable for all your modern day application development. Its has build in supports for plug and play injectable modules without impacting application running sessions. Its highly scalable and easly customizable as your need. ZNxtApp uses NoSql database for lightning performance, easy devlopment of WebApis and support dynamic/arbitrary data models.

**Let's join us on the next generation web development journey, it's free and open source.**

# Quick Start:
### Prerequisites
  - .Net 4.5.2 
  - MongoDB 3.x
### Installation
  - Create new empty WebApplication project from your Visual Studio. (Make sure project .net  version 4.5.2)
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
 - Installation will take few minute. After installation you are ready to use ZNxtApp first application
 - Start your journey. Next create your own module ->>  
  
 Please visit our website [www.ZNxtApp.com](http://ZNxtApp.com/)
 
 License
----

>>>>>>> 9cad77b3bc50422fdfd27401060db794fd5b567a
GNU


**Free Software!**

