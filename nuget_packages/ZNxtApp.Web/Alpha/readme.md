ZNxtApp.Web nuget package


App Settings 

    <add key="Environment" value="prod" />
    <add key="AppName" value="ZNxtAppDummyApp" />
    <add key="AppId" value="b758bd68-e295-4088-a340-5114761c66e4" />
    <add key="AppPath" value="/ZNxtApp.Core.WebTest" />
    <!-- AppMode values  :  Maintance, Debug, Live -->
    <add key="AppMode" value="Maintance" />
    <add key="DataBaseName" value="ZNxtAppTest" />
    <add key="EncryptionKey" value="sscYyr+k1EjnpNoZnil2S6o67zaRWAaEdGVzdF8wYzhlY" />
    <add key="HashKey" value="F8wYzhlYzdhZi1hOTIwLTQ5MWItODcyOC0yYzJhMzk2Z" />
    <add key="MongoDBConnectionString" value="mongodb://localhost:27017" />
   <!-- StaticContentCache : true/false -->
    <add key="StaticContentCache" value="false"/>
    <add key="ModuleCachePath" value="C:\temp\ZNxtApp" />
    <add key="BackendPath" value="/admin001" />
    <add key="DefaultPage" value="/index.z" />
	
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
