using Newtonsoft.Json.Linq;
using System;
using System.Web;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {
        private IAppInstaller _appInstaller;

        public override void ProcessRequest(HttpContext context)
        {
            try
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
                    catch (Exception)
                    {
                        throw;
                    }
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
                data["StackTrace"] = ex.StackTrace;
                _httpProxy.SetResponse(CommonConst._500_SERVER_ERROR, data);
            }
            finally
            {
                WriteResponse();
            }
        }

        private void HandleRequest(string requestUriPath)
        {
            var route = _routings.GetRoute(_httpProxy.GetHttpMethod(), requestUriPath);
            if (route != null)
            {
                _routeExecuter.Exec(route, _httpProxy);
            }
            else
            {
                HandleStaticContent(requestUriPath);
            }
        }

        private void CreateInstallInstance()
        {
            var dbProxy = new MongoDBService();
            var appInstallerLogger = Logger.GetLogger(typeof(ModuleInstaller.Installer.ModuleInstaller).Name, _httpProxy.TransactionId, dbProxy);
            var pingService = new PingService(new MongoDBService());
            var routings = ZNxtApp.Core.Web.Routings.Routings.GetRoutings();
            _appInstaller = ZNxtApp.Core.AppInstaller.Installer.GetInstance(
               pingService,
               new Helpers.DataBuilderHelper(),
               appInstallerLogger,
               dbProxy,
               new EncryptionService(),
               new ModuleInstaller.Installer.ModuleInstaller(appInstallerLogger, dbProxy),
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