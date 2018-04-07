using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Web;
using System.Web.SessionState;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.ModuleInstaller.Installer;
using ZNxtApp.Core.Web.Services;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {

        private IAppInstaller _appInstaller;

        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);

            var requestUriPath = _httpProxy.GetURIAbsolutePath().ToLower();
            requestUriPath = ManagePageUrl(requestUriPath);

            if (ApplicationMode.Maintenance == ApplicationConfig.GetApplicationMode)
            {
                try
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
                catch (Exception ex)
                {
                    // TODO need to handle it better 
                    _logger.Error(ex.Message, ex);
                    JObject data = new JObject();
                    data["Error"] = ex.Message;
                    _httpProxy.SetResponse(CommonConst._500_SERVER_ERROR, data);
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
            //else if (requestUriPath.Contains("uninstall"))
            //{
            //    IModuleUninstaller uninstall = new Uninstaller(_logger, new MongoDBService(ApplicationConfig.DataBaseName));
            //    uninstall.Uninstall("ZNxtApp.Base", _httpProxy);
            //    _httpProxy.SetResponse(200, "Uninstall");
            //}
            else
            {
                HandleStaticContent(requestUriPath);
            }
        }

        private void CreateInstallInstance()
        {
            var appInstallerLogger = Logger.GetLogger(typeof(Installer).Name, _httpProxy.TransactionId);
            var dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            var pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));
            var routings = ZNxtApp.Core.Web.Routings.Routings.GetRoutings();
            _appInstaller = ZNxtApp.Core.AppInstaller.Installer.GetInstance(
               pingService,
               new Helpers.DataBuilderHelper(),
               appInstallerLogger,
               dbProxy,
               new EncryptionService(),
               new ModuleInstaller.Installer.Installer(appInstallerLogger, dbProxy),
              routings);

        }

        private string ManagePageUrl(string pageUri)
        {
            return SetDefaultPage(pageUri);
        }
        private string SetDefaultPage(string pageUri)
        {
            if (pageUri == "/")
            {
                return ApplicationConfig.AppDefaultPage;
            }
            else
            {
                return pageUri;
            }
        }
    }
}