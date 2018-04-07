ZNxtApp.Web nuget package


<appSettings>
    <!-- Put you application name here -->
    <add key="AppName" value="ZNxtAppDummyApp" />
    <!-- this is the unique key of your application. Should be unique in your deploped enviorments  -->
    <add key="AppId" value="b758bd68-e295-4088-a340-5114761c66e4" />
    <!-- This is required in case if you are running application under some virtual directory in IIS-->
    <add key="AppPath" value="/ZNxtApp.Core.WebTest" />
    <!-- ZNxtApp required write access to this path -->
    <add key="ModuleCachePath" value="C:\temp\ZNxtApp" />
    <!-- Mongo db name should be unique to that server-->
    <add key="DataBaseName" value="ZNxtAppTest" />
    <!-- mongoDB connection string-->
    <add key="MongoDBConnectionString" value="mongodb://localhost:27017" />
    <!-- AppMode values  :  Maintance, Debug, Live -->
    <add key="AppMode" value="Maintance" />
</appSettings>	
	----------------------
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
	-----------------
	
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
