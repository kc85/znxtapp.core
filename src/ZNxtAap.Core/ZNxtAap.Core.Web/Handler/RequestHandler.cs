using System;
using System.Net;
using System.Web;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.ModuleInstaller.Installer;
using ZNxtAap.Core.Web.Services;
using ZNxtAap.Core.Web.Util;

namespace ZNxtAap.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {
        private IAppInstaller _appInstaller;
     
        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);

            var requestUriPath = _httpProxy.GetURIAbsolutePath();

            if (ApplicationMode.Maintance == ApplicationConfig.GetApplicationMode)
            {
                CreateInstallInstance();
                if (_appInstaller.Status != Enums.AppInstallStatus.Finish)
                {
                    _appInstaller.Install(_httpProxy);
                }
                else
                {
                    HandleRequest(requestUriPath);
                }
            }
            else
            {
                HandleRequest(requestUriPath);
            }
            WriteResponse();
        }

        private void HandleRequest(string requestUriPath)
        {
            var route = _routings.GetRoute(_httpProxy.GetHttpMethod(), requestUriPath);
            if (route != null)
            {
                _routeExecuter.Exec(route, _httpProxy);
            }
            else if (requestUriPath.Contains("uninstall"))
            {
                IModuleUninstaller uninstall = new Uninstaller(_logger, new MongoDBService(ApplicationConfig.DataBaseName));
                uninstall.Uninstall("ZNxtApp.Base", _httpProxy);
                _httpProxy.SetResponse(200, "Uninstall");
            }
            else
            {
                HandleStaticContent(requestUriPath);
            }
        }

        private void CreateInstallInstance()
        {
            var appInstallerLogger = Logger.GetLogger(typeof(Installer).Name);
            var dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            var pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));

            _appInstaller =  ZNxtAap.CoreAppInstaller.Installer.GetInstance(
               pingService,
               new Helpers.DataBuilderHelper(),
               appInstallerLogger,
               dbProxy,
               new EncryptionService(),
               new ModuleInstaller.Installer.Installer(appInstallerLogger, dbProxy));

        }
    }
}